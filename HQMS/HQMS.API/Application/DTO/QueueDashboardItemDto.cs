using HQMS.API.Domain.Enum;

namespace HQMS.API.Application.DTO
{
    public class QueueDashboardItemDto
    {
        public Guid QueueId { get; set; }
        public string QueueNumber { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string DoctorName { get; set; } = string.Empty;
        public DateTime AppointmentTime { get; set; }
        public string Department { get; set; } = string.Empty;
        public string HospitalName { get; set; }= string.Empty; // Added HospitalName property
        public QueueStatus Status { get; set; }
    }
}
