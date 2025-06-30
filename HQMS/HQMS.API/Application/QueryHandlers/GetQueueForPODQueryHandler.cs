using HQMS.API.Domain.Interfaces;
using HQMS.Application.QuerieModel;
using HQMS.Domain.Entities;
using HQMS.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.Application.QueryHandlers
{
    public class GetQueueForPODQueryHandler : IRequestHandler<GetQueueForPODQuery, List<QueueEntry>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetQueueForPODQueryHandler> _logger;

        public GetQueueForPODQueryHandler(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<GetQueueForPODQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<QueueEntry>> Handle(GetQueueForPODQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"QueueForPOD:{request.DoctorId}";

            // ✅ Check cache first
            var cached = await _cacheService.GetAsync<List<QueueEntry>>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("✅ Queue for POD returned from cache for DoctorId: {DoctorId}", request.DoctorId);
                return cached;
            }

            // ❌ Fetch from DB if not cached
            var queue = await _unitOfWork.QueueRepository.GetQueueByDoctorIdAsync(request.DoctorId);

            // ✅ Store in cache for 2 minutes
            await _cacheService.SetAsync(cacheKey, queue, TimeSpan.FromMinutes(2));

            return queue;
        }
    }
}
