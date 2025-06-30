using HQMS.API.Application.DTO;
using HQMS.API.Application.QueriesMode;
using HQMS.API.Domain.Interfaces;
using MediatR;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetQueueDashboardQueryHandler : IRequestHandler<GetQueueDashboardQuery, List<QueueDashboardItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public GetQueueDashboardQueryHandler(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<List<QueueDashboardItemDto>> Handle(GetQueueDashboardQuery request, CancellationToken cancellationToken)
        {
            // 1. Create a unique cache key based on parameters
            var doctorIdsKey = string.Join(",", request.DoctorIds ?? new List<Guid>());
            var cacheKey = "QueueDashboard_CacheKey";

            // 2. Try to get cached data
            var cached = await _cacheService.GetAsync<List<QueueDashboardItemDto>>(cacheKey);
            if (cached != null)
                return cached;

            // 3. If not cached, fetch from DB
            var result = await _unitOfWork.QueueRepository
                .GetDashboardDataAsync(request.HospitalId, request.DepartmentId, request.DoctorIds);

            // 4. Cache the result
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromSeconds(10)); // ⏱️ You can adjust expiration

            return result;
        }
    }
}
