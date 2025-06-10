using HQMS.API.Domain.Entities;
using System.Numerics;

namespace HospitalQueueSystem.Domain.Entities
{
    public class DoctorQueue
    {
        public int Id { get; set; }

        public Guid DoctorId { get; set; } // Use Guid to match Doctor entity
        public Doctor Doctor { get; set; } // Add navigation property

        public string DoctorName { get; set; } = string.Empty;
        public int CurrentToken { get; set; }
        public int QueueNumber { get; set; }
        public string StartingToken { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

}
