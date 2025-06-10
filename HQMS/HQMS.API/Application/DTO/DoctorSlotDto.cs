namespace HQMS.API.Application.DTO
{
    public class DoctorSlotDto
    {
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
