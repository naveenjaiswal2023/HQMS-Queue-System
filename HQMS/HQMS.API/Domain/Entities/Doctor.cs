using HospitalQueueSystem.Domain.Entities;
using HQMS.API.Domain.Entities;

namespace HQMS.API.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public Guid Id { get; set; } // Primary key

        // Basic Info
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        // Professional Info
        public string Specialization { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int ExperienceInYears { get; set; }

        // Availability
        public bool IsAvailable { get; set; } = true;
        public string ConsultationType { get; set; } = "InPerson"; // InPerson, Online, Both

        // Hospital Info
        public Guid HospitalId { get; set; } // Foreign key to Hospital (if applicable)

        // Navigation properties
        public ICollection<DoctorSlot> DoctorSlots { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<DoctorQueue> DoctorQueues { get; set; } = new List<DoctorQueue>();
    }
}
