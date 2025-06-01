using Azure.Identity;
using HospitalQueueSystem.Web.Extensions;
using HospitalQueueSystem.Web.Hubs;
using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HospitalQueueSystem.Web.Services;
using HQMS.UI.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
//builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

// Add Azure Key Vault secrets if VaultUrl is configured
var keyVaultUrl = builder.Configuration["AzureKeyVault:VaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    var credential = new DefaultAzureCredential();

    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUrl), credential);
}

// Final configuration object (after Key Vault is loaded)
var configuration = builder.Configuration;

// Extract secrets
var blobStorageConnectionString = configuration["BlobStorageConnectionString"];
var blobContainerName = configuration["Logging:BlobStorage:ContainerName"];

// Configure Serilog with Azure Blob Storage
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.AzureBlobStorage(
        connectionString: blobStorageConnectionString,
        storageContainerName: blobContainerName,
        storageFileName: "log-{yyyyMMdd}.txt",
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}",
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();


builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IPatientService, PatientService>();
builder.Services.AddHttpContextAccessor();


// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
    });
builder.Services.AddSignalR();

// Add HttpClient
builder.Services.AddHttpClient();

// Add Session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Always in production
});

// Add Cookie Authentication for MVC
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Use Always in production
    });

// Add Authorization
builder.Services.AddAuthorization();

// Add your API service
builder.Services.AddApiService();

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "https://localhost:3000",
                "https://localhost:4200",
                "https://yourdomain.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
// 🛡️ Global Exception Handling (should be FIRST to catch all exceptions)
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseStaticFiles();

// Add Session middleware
builder.Services.AddSession();
app.UseSession();

app.UseRouting();

app.UseCors();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// Map SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

app.Run();