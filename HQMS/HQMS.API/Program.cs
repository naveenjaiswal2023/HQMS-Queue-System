using AspNetCoreRateLimit;
using Azure.Identity;
using Azure.Messaging.ServiceBus;
using HQMS.Application.Common;
using HQMS.Application.Handlers;
using HQMS.Domain.Entities;
using HQMS.Domain.Events;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using HQMS.Infrastructure.Events;
using HQMS.Infrastructure.SignalR;
using HQMS.Shared.Utilities;
using HQMS.WebAPI.Controllers;
using HospitalQueueSystem.WebAPI.Middleware;
using HQMS.API.Application;
using HQMS.API.Application.DTO;
using HQMS.API.Application.Services;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Infrastructure;
using HQMS.API.Infrastructure.Repositories;
using HQMS.API.WebAPI.Controllers;
using HQMS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Polly;
using Serilog;
using Serilog.Events;
using System.Text;
using HQMS.Application.DTO;
using HQMS.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Bind base + environment-specific appsettings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var environment = builder.Environment;
var isDevelopment = environment.IsDevelopment();

// Add Azure Key Vault only in non-dev environments
if (!isDevelopment)
{
    var keyVaultUrl = builder.Configuration["AzureKeyVault:VaultUrl"];
    if (!string.IsNullOrEmpty(keyVaultUrl))
    {
        try
        {
            var credential = new DefaultAzureCredential();
            builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), credential);
            Console.WriteLine("✅ Azure Key Vault loaded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to load Azure Key Vault: {ex.Message}");
        }
    }
    else
    {
        Console.WriteLine("ℹ️ Azure Key Vault URL not configured.");
    }
}

// Extract settings after full config is loaded
var configuration = builder.Configuration;
var blobStorageConnectionString = configuration["BlobStorageConnectionString"];
var blobContainerName = configuration["Logging:BlobStorage:ContainerName"];
var dbPassword = configuration["QmsDbPassword"];
var connTemplate = configuration.GetConnectionString("DefaultConnection");
var actualConnectionString = connTemplate?.Replace("_QmsDbPassword_", dbPassword);
var azureServiceBusConnectionString = configuration["AzureServiceBusConnectionString"];
// Bind strongly typed configs
builder.Services.Configure<MaintenanceModeOptionsDto>(
    configuration.GetSection("MaintenanceMode"));

builder.Services.Configure<ExternalApiOptions>(
    configuration.GetSection("ExternalApi"));

// Register app services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);

// Configure Serilog logging
try
{
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration.MinimumLevel.Information()
            .WriteTo.Console();

        if (isDevelopment)
        {
            loggerConfiguration.WriteTo.File(
                path: "Logs/hqms-api-log-.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Information);
            Console.WriteLine("📄 Serilog writing to local file (Development).");
        }
        else
        {
            loggerConfiguration.WriteTo.AzureBlobStorage(
                connectionString: blobStorageConnectionString,
                storageContainerName: blobContainerName,
                storageFileName: "hqms-api-log-{yyyyMMdd}.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information);
            Console.WriteLine("☁️ Serilog writing to Azure Blob Storage (Production).");
        }

        loggerConfiguration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services);
    });

    Console.WriteLine("✅ Serilog configured.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Serilog configuration failed: {ex.Message}");
}

// Add environment variables
builder.Configuration.AddEnvironmentVariables();

// Register DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // Use the dynamically generated connection string with retry logic
    options.UseSqlServer(actualConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,                         // Maximum retry attempts
            maxRetryDelay: TimeSpan.FromSeconds(30), // Retry delay
            errorNumbersToAdd: null);                // Optional: additional error codes to retry
    });
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtDto>();
builder.Services.Configure<JwtDto>(builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// Azure Service Bus connection
//var serviceBusConnectionString = Environment.GetEnvironmentVariable("SERVICEBUS_CONNECTIONSTRING");

if (!string.IsNullOrEmpty(azureServiceBusConnectionString))
{
    builder.Configuration["AzureServiceBus:ConnectionString"] = azureServiceBusConnectionString;
}

// Register ServiceBusClient (Singleton)
builder.Services.AddSingleton(serviceProvider =>
{
    var connectionString = builder.Configuration["AzureServiceBus:ConnectionString"];
    return new ServiceBusClient(connectionString);
});

// Register ServiceBusSender for patient-topic (Singleton)
builder.Services.AddSingleton(serviceProvider =>
{
    var serviceBusClient = serviceProvider.GetRequiredService<ServiceBusClient>();
    return serviceBusClient.CreateSender("patient-topic"); // Specify the topic name here
});

// Register ServiceBusSender for doctor-queue topic (Singleton)
builder.Services.AddSingleton(serviceProvider =>
{
    var serviceBusClient = serviceProvider.GetRequiredService<ServiceBusClient>();
    return serviceBusClient.CreateSender("doctor-queue"); // Specify the topic name here
});

// Register topics/subscriptions as List<TopicSubscriptionPair>
builder.Services.AddSingleton(new List<TopicSubscriptionPair>
{
    new TopicSubscriptionPair { TopicName = "patient-topic", SubscriptionName = "qms-subscription" },
    new TopicSubscriptionPair { TopicName = "doctor-queue", SubscriptionName = "doctor-queue-subscription" }
});

// Event Handlers
builder.Services.AddScoped<DoctorQueueCreatedEventHandler>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<RegisterPatientCommandHandler>();
});

// Rate Limiting
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

//builder.Services.AddHttpClient<IExternalHospitalService, ExternalHospitalService>((sp, client) =>
//{
//    var config = sp.GetRequiredService<IOptions<ExternalApiOptions>>().Value;
//    client.BaseAddress = new Uri(config.BaseUrl);
//    client.DefaultRequestHeaders.Add("Accept", "application/json");
//})
//.AddTransientHttpErrorPolicy(policyBuilder =>
//    policyBuilder.WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
//.AddTransientHttpErrorPolicy(policyBuilder =>
//    policyBuilder.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

// CORS

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://localhost:7026",      // local dev server
                "https://hqms-ui-bwgnfqd6f6abg0es.eastasia-01.azurewebsites.net"      // Azure dev server
                //"https://yourdomain.com",      // Production frontend
                //"https://www.yourdomain.com",  // Production frontend with www
                //"https://staging.yourdomain.com" // Staging environment
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Hospital Queue System API",
        Version = "v1"
    });

    //Define the Bearer scheme
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsIn..."
    });

    //Apply the scheme globally
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Controllers
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await IdentitySeed.SeedRolesAndAdminAsync(services);
}
// Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 🛡️ Global Exception Handling (should be FIRST to catch all exceptions)
app.UseMiddleware<GlobalExceptionMiddleware>();

// 🌐 HTTPS Redirection (early in pipeline for security)
app.UseHttpsRedirection();

// 🛠️ Maintenance Mode Check (early check before processing requests)
app.UseMaintenanceMode();

// 🚦 Rate Limiting (protect your API early, before expensive operations)
app.UseIpRateLimiting();

// 🌐 Routing (establishes route context)
app.UseRouting();

// 🌍 CORS (after routing, before auth - needs route context)
app.UseCors();

// 🔐 Authentication (must come before authorization)
app.UseAuthentication();

// 🔐 Custom Unauthorized Middleware (after auth, before authorization)
// app.UseUnauthorizedMiddleware();

// 🔐 Authorization (must come after authentication)
app.UseAuthorization();

// 🚀 Response Caching (after auth/authz, before controllers)
// app.UseCachedResponse();

// 🧭 Endpoints (final step - actual request processing)
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<NotificationHub>("/NotificationHub");
app.MapGet("/", () => Results.Ok("🏥 Hospital Queue System API is running"));

// Database migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

    app.Run();

public partial class Program { }  // This partial class is needed for WebApplicationFactory<T>