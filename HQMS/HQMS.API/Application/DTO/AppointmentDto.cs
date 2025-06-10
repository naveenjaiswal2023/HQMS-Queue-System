namespace HQMS.API.Application.DTO
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }
        public DateTime AppointmentTime { get; set; }
    }
}
