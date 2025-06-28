
using HQMS.API.Application.DTO;
using HQMS.API.Application.QueriesMode;
using MediatR;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetQueueDashboardQueryHandler : IRequestHandler<GetQueueDashboardQuery, List<QueueDashboardItemDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetQueueDashboardQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QueueDashboardItemDto>> Handle(GetQueueDashboardQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.QueueRepository.GetDashboardDataAsync(request.HospitalId, request.DepartmentId, request.DoctorIds);
        }

    }
}
