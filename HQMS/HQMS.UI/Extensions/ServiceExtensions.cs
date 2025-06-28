
using HQMS.Web.Services;

namespace HQMS.Web.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApiService(this IServiceCollection services)
        {
            services.AddScoped<ApiService>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
