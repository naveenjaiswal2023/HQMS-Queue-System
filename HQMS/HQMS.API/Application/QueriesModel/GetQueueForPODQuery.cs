using HQMS.Domain.Entities;
using MediatR;

namespace HQMS.Application.QuerieModel
{
    public class GetQueueForPODQuery : IRequest<List<QueueEntry>>
    {
        public int DoctorId { get; set; }
    }

}
