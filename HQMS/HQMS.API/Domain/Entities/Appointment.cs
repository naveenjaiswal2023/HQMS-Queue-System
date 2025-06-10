using HospitalQueueSystem.Domain.Entities;
using System.Numerics;

namespace HQMS.API.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid HospitalId { get; set; }

        public DateTime AppointmentTime { get; set; }
        public string Status { get; set; } = "Scheduled";
        public string Notes { get; set; }

        // Navigation Properties
        public Patient Patient { get; set; }
        public Doctor Doctor { get; set; }
        public Hospital Hospital { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }


}
