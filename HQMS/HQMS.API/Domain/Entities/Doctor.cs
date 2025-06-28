using HQMS.Domain.Entities;
using HQMS.Domain.Entities.Common;


namespace HQMS.API.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        public Guid Id { get; set; }  // Primary Key

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;
        public string Qualification { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public int ExperienceInYears { get; set; }

        public bool IsAvailable { get; set; } = true;
        public string ConsultationType { get; set; } = "InPerson";

        public Guid HospitalId { get; set; }
        public Guid? DepartmentId { get; set; }
        // Navigation properties
        public ICollection<DoctorSlot> DoctorSlots { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorQueue> DoctorQueues { get; set; } = new List<DoctorQueue>();
        
        public Department Department { get; set; }
        public Hospital Hospital { get; set; }

    }
}
