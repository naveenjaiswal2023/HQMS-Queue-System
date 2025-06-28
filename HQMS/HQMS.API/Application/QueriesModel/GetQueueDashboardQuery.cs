using HQMS.API.Application.DTO;
using MediatR;

namespace HQMS.API.Application.QueriesMode
{
    public class GetQueueDashboardQuery : IRequest<List<QueueDashboardItemDto>>
    {
        public Guid? HospitalId { get; set; }
        public Guid? DepartmentId { get; set; }
        public IEnumerable<Guid> DoctorIds { get; set; }
    }
}
