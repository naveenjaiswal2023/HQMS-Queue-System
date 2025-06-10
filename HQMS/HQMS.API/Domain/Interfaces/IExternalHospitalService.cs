using HQMS.API.Application.DTO;

namespace HQMS.API.Domain.Interfaces
{
    public interface IExternalHospitalService
    {
        //Task<HospitalDto> GetHospitalByIdAsync(Guid id);
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<List<DoctorSlotDto>> GetDoctorSlotsAsync(Guid doctorId);
        Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId);
    }

}
