using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using HospitalQueueSystem.Web.Extensions;
using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HospitalQueueSystem.Web.Services;
using HQMS.UI.Handlers;
using HQMS.UI.Middlewares;
using HQMS.UI.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure strongly typed settings
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

builder.Services.Configure<SignalRSettings>(builder.Configuration.GetSection("SignalR"));

var environment = builder.Environment.EnvironmentName;
var isDevelopment = builder.Environment.IsDevelopment();

// 1. Load base + environment-specific config
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

if (!builder.Environment.IsDevelopment())
{
    // Add Azure Key Vault secrets if VaultUrl is configured
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

// Final configuration object (after Key Vault is loaded)
var configuration = builder.Configuration;

// Extract secrets
var blobStorageConnectionString = configuration["BlobStorageConnectionString"];
var blobContainerName = configuration["Logging:BlobStorage:ContainerName"];

// Configure Serilog
try
{
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.AzureBlobStorage(
                connectionString: blobStorageConnectionString,
                storageContainerName: blobContainerName,
                storageFileName: "hqms-ui-log-{yyyyMMdd}.txt",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
                restrictedToMinimumLevel: LogEventLevel.Information)
            .ReadFrom.Configuration(context.Configuration)     //  Required for appsettings.json logging config
            .ReadFrom.Services(services);                      //  Required to register DiagnosticContext
    });

    Console.WriteLine("✅ Serilog configured.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Serilog configuration failed: {ex.Message}");
}


// DI registrations
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]);
})
.AddHttpMessageHandler<AuthorizationHandler>();

// Register AuthorizationHandler
builder.Services.AddTransient<AuthorizationHandler>();

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPatientService, PatientService>();


builder.Services.AddControllersWithViews(options =>
{
    // Automatically apply anti-forgery token validation to all POST actions
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
})
.AddViewOptions(options =>
{
    options.HtmlHelperOptions.ClientValidationEnabled = true;
});


builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});


builder.Services.AddSignalR();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();

// Register custom API service extension
builder.Services.AddApiService();
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

// Global exception middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    await next();
});

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});


app.UseRouting();

// Remove or configure this properly
// app.UseCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// Optional: log requests with Serilog
app.UseSerilogRequestLogging();

// MVC routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
public partial class Program { }
