using HQMS.API.Domain.Entities;
using HQMS.Domain.Events;

namespace HQMS.API.Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<List<PatientRegisteredEvent>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    }
}
