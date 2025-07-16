using HQMS.UI.Handlers;
using HQMS.UI.Interfaces;
using HQMS.UI.Middlewares;
using HQMS.UI.Models;
using HQMS.UI.Services;
using HQMS.Web.Extensions;
using HQMS.Web.Interfaces;
using HQMS.Web.Models;
using HQMS.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Load base + env config + environment variables
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var configuration = builder.Configuration;
var environment = builder.Environment;

// Get secrets based on environment
string blobStorageConnectionString = "";
string blobContainerName = "";

if (environment.IsDevelopment())
{
    blobStorageConnectionString = configuration["BlobStorageConnectionString"];
    blobContainerName = configuration["Logging:BlobStorage:ContainerName"];
}
else
{
    // Get from environment variables (Azure App Service > Configuration)
    blobStorageConnectionString = Environment.GetEnvironmentVariable("BlobStorageConnectionString");
    blobContainerName = Environment.GetEnvironmentVariable("BlobStorageContainerName");
}

// Configure Serilog
try
{
    builder.Host.UseSerilog((context, services, loggerConfiguration) =>
    {
        loggerConfiguration.MinimumLevel.Information()
            .WriteTo.Console();

        if (environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.File(
                path: "Logs/hqms-ui-log-.txt",
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
                storageFileName: "hqms-ui-log-{yyyyMMdd}.txt",
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

// Register DI services
builder.Services.Configure<ApiSettings>(configuration.GetSection("ApiSettings"));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHttpClient("AuthorizedClient", client =>
{
    client.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"]);
})
.AddHttpMessageHandler<AuthorizationHandler>();

builder.Services.AddTransient<AuthorizationHandler>();

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IQueueService, QueueService>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
})
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
builder.Services.AddApiService();

var app = builder.Build();

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseRouting();
// app.UseCors(); // enable if needed
app.UseAuthentication();
app.UseAuthorization();

app.UseStatusCodePages(async context =>
{
    if (context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Redirect("/Auth/Login");
    }
});

app.UseSession();
app.UseSerilogRequestLogging();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

public partial class Program { }
