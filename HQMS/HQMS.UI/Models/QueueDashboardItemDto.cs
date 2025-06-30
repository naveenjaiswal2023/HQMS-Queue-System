using HQMS.UI.Enum;

namespace HQMS.UI.Models
{
    public class QueueDashboardItemDto
    {
        public Guid QueueId { get; set; }
        public string QueueNumber { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public DateTime AppointmentTime { get; set; }
        public string Department { get; set; }
        public string HospitalName { get; set; }
        public QueueStatus Status { get; set; }
    }
}
