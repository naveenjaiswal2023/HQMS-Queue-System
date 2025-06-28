using Azure.Messaging.ServiceBus;
using FluentValidation;
using HQMS.API.Application.Services;
using HQMS.API.Application.Validators;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.API.Infrastructure.Repositories;
using HQMS.API.WebAPI.Controllers;
using HQMS.Application.Common;
using HQMS.Application.Services;
using HQMS.Domain.Interfaces;
using HQMS.Infrastructure.Data;
using HQMS.Infrastructure.Events;
using HQMS.Infrastructure.Repositories;
using HQMS.Shared.Utilities;
using HQMS.WebAPI.Controllers;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 📦 Database Context
            //services.AddDbContext<StaffDbContext>(options =>
            //    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // ⚡ Redis Cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"];
            });

            // 🔄 Azure Service Bus
            services.AddSingleton<ServiceBusClient>(sp =>
            {
                var connectionString = configuration.GetConnectionString("ServiceBus")
                                       ?? configuration["AzureServiceBus:ConnectionString"];
                return new ServiceBusClient(connectionString);
            });

            // 📚 Repositories
            //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            //services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddHostedService<AzureBusBackgroundService>();
            services.AddScoped<IDomainEventPublisher, DomainEventPublisher>();

            services.AddHostedService<HospitalDataSyncService>();
            services.AddHostedService<QueueScheduler>();

            // Register IHttpContextAccessor (for accessing user info in services)
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IUserContextService, UserContextService>();

            // SignalR
            services.AddSignalR();
            services.AddSingleton<INotificationService, NotificationService>();

            // In-Memory Caching
            services.AddMemoryCache(); // Register IMemoryCache
            
            // 🔧 Services
            services.AddSingleton<ICacheService, CacheService>();
            services.AddValidatorsFromAssembly(typeof(LoginCommandValidator).Assembly);
            services.AddValidatorsFromAssemblyContaining<CreateRoleCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<RegisterPatientCommandValidator>();
            services.AddValidatorsFromAssemblyContaining<UpdatePatientCommandValidator>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<PatientController>();
            services.AddScoped<RolesController>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IRepository<Menu>, MenuRepository>();
            services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();
            services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            // Fix for CS0311: Ensure that DoctorQueueRepository implements IDoctorQueueRepository
            services.AddScoped<IDoctorQueueRepository, DoctorQueueRepository>();

            //services.AddScoped<IQueueServiceClient, QueueServiceClient>();
            //services.AddScoped<IHttpClientService, HttpClientService>();
            //services.AddScoped<ICurrentUserService, CurrentUserService>();
            //services.AddHttpContextAccessor();
            //services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<INotificationService, NotificationService>();
            //services.AddScoped<ICacheService, CacheService>();
            //services.AddScoped<IQueueRepository, QueueRepository>();

            //services.AddHttpClient("QueueService", (serviceProvider, client) =>
            //{
            //    var config = serviceProvider.GetRequiredService<IConfiguration>();
            //    client.BaseAddress = new Uri(config["ServiceUrls:QueueService"]);
            //});

            //services.AddHttpClient("PatientService", (serviceProvider, client) =>
            //{
            //    var config = serviceProvider.GetRequiredService<IConfiguration>();
            //    client.BaseAddress = new Uri(config["ServiceUrls:PatientService"]);
            //});

            // ✅ Generic reusable HttpClient for internal services
            //services.AddHttpClient<IHttpClientService, HttpClientService>();

            return services;
        }
    }
}
