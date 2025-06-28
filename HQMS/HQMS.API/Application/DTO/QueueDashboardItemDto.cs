namespace HQMS.API.Application.DTO
{
    public class QueueDashboardItemDto
    {
        public string QueueNumber { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentTime { get; set; }
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; }
    }
}
