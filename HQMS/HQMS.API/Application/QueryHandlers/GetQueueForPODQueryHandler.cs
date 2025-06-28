
using HQMS.Application.QuerieModel;
using HQMS.Domain.Entities;
using MediatR;

namespace HQMS.Application.QueryHandlers
{
    public class GetQueueForPODQueryHandler : IRequestHandler<GetQueueForPODQuery, List<QueueEntry>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetQueueForPODQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<QueueEntry>> Handle(GetQueueForPODQuery request, CancellationToken cancellationToken)
        {

            return await _unitOfWork.QueueRepository.GetQueueByDoctorIdAsync(request.DoctorId);
        }
    }

}
