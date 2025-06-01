using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using HospitalQueueSystem.Web.Extensions;
using HospitalQueueSystem.Web.Hubs;
using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using HospitalQueueSystem.Web.Services;
using HQMS.UI.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure strongly typed settings
builder.Services.Configure<ApiSettings>(
    builder.Configuration.GetSection("ApiSettings"));

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

// Final configuration object (after Key Vault is loaded)
var configuration = builder.Configuration;

// Extract secrets
var blobStorageConnectionString = configuration["BlobStorageConnectionString"];
var blobContainerName = configuration["Logging:BlobStorage:ContainerName"];

// Configure Serilog
try
{
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

    builder.Host.UseSerilog();
    Console.WriteLine("✅ Serilog configured.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Serilog configuration failed: {ex.Message}");
}

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpClient<IPatientService, PatientService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

builder.Services.AddControllersWithViews()
    .AddViewOptions(options =>
    {
        options.HtmlHelperOptions.ClientValidationEnabled = true;
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

// Optional: configure CORS
// builder.Services.AddCors(options =>
// {
//     options.AddDefaultPolicy(policy =>
//     {
//         policy.WithOrigins("https://localhost:3000", "https://localhost:4200")
//               .AllowAnyHeader()
//               .AllowAnyMethod()
//               .AllowCredentials();
//     });
// });

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

// SignalR hub
app.MapHub<NotificationHub>("/notificationHub");

app.Run();
