using HQMS.API.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<List<Appointment>> GetAppointmentsWithinNextMinutesAsync(int minutes);
        Task<Appointment?> GetByDoctorAndPatientAsync(Guid doctorId, Guid patientId);
        Task<List<Appointment>> GetAppointmentsForDoctorByDateAsync(Guid doctorId, DateTime date);


    }
}
