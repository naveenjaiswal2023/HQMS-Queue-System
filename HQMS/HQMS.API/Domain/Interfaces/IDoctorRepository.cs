using HQMS.API.Domain.Entities;
using HQMS.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IDoctorRepository : IRepository<Doctor>
    {
        //Task<QueueItem?> GetByDoctorIdAsync(Guid doctorId);
    }
}
