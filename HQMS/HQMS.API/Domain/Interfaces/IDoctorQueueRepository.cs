
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Entities;

namespace HQMS.Domain.Interfaces
{
    public interface IDoctorQueueRepository : IRepository<DoctorQueue>
    {
        Task<DoctorQueue?> GetByDoctorIdAsync(Guid doctorId);
    }
}
