using HQMS.Domain.Events;

namespace HQMS.Domain.Interfaces
{
    public interface IPatientCacheService
    {
        Task AddPatientToCacheAsync(PatientRegisteredEvent patient);
        Task<List<PatientRegisteredEvent>> GetQueueAsync();
    }
}
