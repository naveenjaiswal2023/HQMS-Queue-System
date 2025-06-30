

using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class Hospital : BaseEntity
    {
        public Guid HospitalId { get; set; } // Primary key

        // Basic Info
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty; // Unique hospital code
        public string Type { get; set; } = string.Empty; // e.g., Multi-specialty, Clinic, Diagnostic Center

        // Address Info
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = "India";
        public string PostalCode { get; set; } = string.Empty;

        // Contact Info
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string HelplineNumber { get; set; } = string.Empty;

        // Capacity Info
        public int TotalBeds { get; set; }
        public int ICUUnits { get; set; }
        public int EmergencyUnits { get; set; }

        // Navigation Properties
        public ICollection<Doctor> Doctors { get; set; }
        public ICollection<Patient> Patients { get; set; }
        public ICollection<Department> Departments { get; set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
