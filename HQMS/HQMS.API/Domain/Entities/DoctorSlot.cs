using System.Numerics;

namespace HQMS.API.Domain.Entities
{
    public class DoctorSlot : BaseEntity
    {
        public Guid Id { get; set; } // Optional if using composite key
        public Guid DoctorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Navigation property
        public Doctor Doctor { get; set; }
    }

}
