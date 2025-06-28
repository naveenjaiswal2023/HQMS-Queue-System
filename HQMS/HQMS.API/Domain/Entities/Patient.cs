using HQMS.Domain.Events;
using HQMS.API.Domain.Entities;
using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Gender { get; private set; }

        public Guid DepartmentId { get; private set; }               // ✅ FK
        public Department Department { get; private set; }           // ✅ Navigation

        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public string BloodGroup { get; private set; }

        public Guid HospitalId { get; private set; }
        public Hospital Hospital { get; private set; }

        public Guid PrimaryDoctorId { get; private set; }
        public Doctor PrimaryDoctor { get; private set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public Patient(string name, int age, string gender, Guid departmentId,
                       string phoneNumber, string email, string address, string bloodGroup,
                       Guid hospitalId, Guid primaryDoctorId)
        {
            PatientId = Guid.NewGuid();
            Name = name;
            Age = age;
            Gender = gender;
            DepartmentId = departmentId;                     // ✅ assign FK
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            PrimaryDoctorId = primaryDoctorId;

            AddDomainEvent(new PatientRegisteredEvent(
                PatientId,
                Name,
                Age,
                Gender,
                Department?.DepartmentName ?? "Unknown",   // ✅ now string
                PhoneNumber,
                Email,
                Address,
                BloodGroup,
                Hospital?.Name ?? "Unknown",     // ✅ now string
                PrimaryDoctor?.FullName ?? "Unknown" // ✅ now string
            ));

        }

        public void UpdateDetails(string name, int age, string gender, Guid departmentId,
                                  string phoneNumber, string email, string address, string bloodGroup,
                                  Guid hospitalId, Guid doctorId, DateTime updatedAt)
        {
            Name = name;
            Age = age;
            Gender = gender;
            DepartmentId = departmentId;                     // ✅ assign FK
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            PrimaryDoctorId = doctorId;
            ModifiedAt = updatedAt;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = "System";

            AddDomainEvent(new PatientUpdatedEvent(PatientId, Name, Age, Gender, departmentId, PhoneNumber, Email, Address, BloodGroup, HospitalId, PrimaryDoctorId));
        }

        public void MarkAsDeleted()
        {
            AddDomainEvent(new PatientDeletedEvent(PatientId));
        }
    }
}
