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
            services.AddScoped<IDoctorQueueRepository, DoctorQueueRepository>();

            return services;
        }
    }
}
