using HospitalQueueSystem.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IQueueRepository : IRepository<QueueEntry>
    {
        Task<List<QueueEntry>> GetQueueByDoctorIdAsync(int doctorId);
    }

}
