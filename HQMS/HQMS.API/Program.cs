using AspNetCoreRateLimit;
using Azure.Messaging.ServiceBus;
using HQMS.Application.Common;
using HQMS.Application.Handlers;
using HQMS.Application.DTO;
using HQMS.Domain.Events;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using HQMS.Infrastructure.Events;
using HQMS.Infrastructure.Repositories;
using HQMS.Infrastructure.Seed;
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

var builder = WebApplication.CreateBuilder(args);

// Load config sources
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var environment = builder.Environment;
var isDevelopment = environment.IsDevelopment();

// ✅ Use env variables in Production, appsettings in Development
string blobStorageConnectionString, dbPassword;
if (environment.IsDevelopment())
{
    blobStorageConnectionString = configuration["BlobStorageConnectionString"];
    dbPassword = configuration["QmsDbPassword"];

}
else
{
    blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
    dbPassword = Environment.GetEnvironmentVariable("QmsDbPassword");
}

var blobContainerName = configuration["Logging:BlobStorage:ContainerName"];
var connTemplate = configuration.GetConnectionString("DefaultConnection");
var actualConnectionString = connTemplate?.Replace("_QmsDbPassword_", dbPassword);
var azureServiceBusConnectionString = configuration["AzureServiceBusConnectionString"];

// Strongly typed config bindings
builder.Services.Configure<ServiceBusSettings>(configuration.GetSection("ServiceBus"));
builder.Services.Configure<MaintenanceModeOptionsDto>(configuration.GetSection("MaintenanceMode"));
builder.Services.Configure<ExternalApiOptions>(configuration.GetSection("ExternalApi"));

// Application + Infrastructure
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(configuration);

// ✅ Serilog Logging
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
    }
    else
    {
        if (!string.IsNullOrWhiteSpace(blobStorageConnectionString))
        {
            loggerConfiguration.WriteTo.AzureBlobStorage(
                connectionString: blobStorageConnectionString,
                storageContainerName: blobContainerName,
                storageFileName: "hqms-api-log-{yyyyMMdd}.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information);
        }
        else
        {
            Console.WriteLine("❌ Missing BlobStorageConnectionString environment variable in production.");
        }
    }

    loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services);
});

// DB Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(actualConnectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
    });
});

// Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// JWT Authentication
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtDto>();
builder.Services.Configure<JwtDto>(configuration.GetSection("JwtSettings"));

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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

// Azure Service Bus
if (!string.IsNullOrEmpty(azureServiceBusConnectionString))
{
    configuration["AzureServiceBus:ConnectionString"] = azureServiceBusConnectionString;
}

builder.Services.AddSingleton(serviceProvider =>
{
    var conn = configuration["AzureServiceBus:ConnectionString"];
    return new ServiceBusClient(conn);
});

var serviceBusSettings = configuration.GetSection("ServiceBus").Get<ServiceBusSettings>();
builder.Services.AddSingleton(serviceBusSettings.TopicSubscriptions);

// Event Handlers
builder.Services.AddScoped<DoctorQueueCreatedEventHandler>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<RegisterPatientCommandHandler>();
});

// Rate Limiting
builder.Services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://localhost:7026",
                "https://hqms-ui-bwgnfqd6f6abg0es.eastasia-01.azurewebsites.net")
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

    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your JWT token.\n\nExample: Bearer eyJhbGciOiJIUzI1NiIsIn..."
    });

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

builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Seed roles + admin
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

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseMaintenanceMode();
app.UseIpRateLimiting();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");
app.MapHub<NotificationHub>("/NotificationHub");
app.MapGet("/", () => Results.Ok("🏥 Hospital Queue System API is running"));

// Auto DB migration
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();
}

app.Run();

public partial class Program { }
