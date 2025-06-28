using HQMS.Domain.Entities;
using HQMS.API.Application.DTO;
using HQMS.API.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IQueueRepository : IRepository<QueueItem>
    {
        Task<List<QueueEntry>> GetQueueByDoctorIdAsync(int doctorId);
        Task<List<QueueItem>> GetWaitingPatientsByDepartmentAsync(Guid departmentId);
        Task<List<QueueItem>> GetQueueItemsForDoctorByDateAsync(Guid doctorId, DateTime date);
        Task<QueueItem?> GetByAppointmentIdAsync(Guid appointmentId);
        Task<int> GetNextPositionAsync(Guid doctorId);

        Task<List<QueueDashboardItemDto>> GetDashboardDataAsync(Guid? hospitalId, Guid? departmentId, IEnumerable<Guid> doctorIds);
    }

}
