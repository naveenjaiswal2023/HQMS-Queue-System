using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using HQMS.Web.Services;

namespace HQMS.UI.Services
{
    public class QueueService : IQueueService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public QueueService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<QueueDashboardItemDto>> GetDashboardAsync(Guid? hospitalId, Guid? departmentId, Guid? doctorId)
        {
            var queryParams = new List<string>();

            if (hospitalId.HasValue)
                queryParams.Add($"hospitalId={hospitalId.Value}");

            if (departmentId.HasValue)
                queryParams.Add($"departmentId={departmentId.Value}");

            if (doctorId.HasValue)
                queryParams.Add($"doctorId={doctorId.Value}");

            var queryString = string.Join("&", queryParams);
            var endpoint = string.IsNullOrEmpty(queryString)
                ? "Queue/GetQueueDashboard"
                : $"Queue/GetQueueDashboard?{queryString}";

            // ✅ FIX: Correct generic type to List<QueueDashboardItemDto>
            var response = await _apiService.GetAsync<List<QueueDashboardItemDto>>(endpoint);

            return response?.Data ?? new List<QueueDashboardItemDto>();
        }
    }
}
