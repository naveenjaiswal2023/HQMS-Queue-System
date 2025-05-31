using HospitalQueueSystem.Web.Models;

namespace HospitalQueueSystem.Web.Interfaces
{
    public interface IPatientService
    {
        Task<List<PatientModel>> GetAllAsync();
        Task<PatientModel> GetByIdAsync(Guid id);
        Task CreateAsync(PatientModel patient);
        Task UpdateAsync(Guid id, PatientModel patient);
        Task DeleteAsync(Guid id);
    }
}
