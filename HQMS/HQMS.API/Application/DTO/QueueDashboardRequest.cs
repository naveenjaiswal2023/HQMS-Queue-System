namespace HQMS.API.Application.DTO
{
    public class QueueDashboardRequest
    {
        public Guid? HospitalId { get; set; }
        public Guid? DepartmentId { get; set; }
        public List<Guid>? DoctorIds { get; set; }
    }
}
