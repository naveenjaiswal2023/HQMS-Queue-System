using HospitalQueueSystem.Domain.Common;
using HospitalQueueSystem.Domain.Events;
using HQMS.API.Domain.Entities;

namespace HospitalQueueSystem.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public string Name { get; private set; }
        public int Age { get; private set; }
        public string Gender { get; private set; }
        public string Department { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public string BloodGroup { get; private set; }

        // Foreign keys
        public Guid HospitalId { get; private set; }
        public Hospital Hospital { get; private set; }

        public Guid PrimaryDoctorId { get; private set; }
        public Guid PrimaryDoctor { get; private set; }
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public Patient(string name, int age, string gender, string department,
                       string phoneNumber, string email, string address, string bloodGroup,
                       Guid hospitalId, Guid primaryDoctorId)
        {
            PatientId = Guid.NewGuid();
            Name = name;
            Age = age;
            Gender = gender;
            Department = department;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            PrimaryDoctorId = primaryDoctorId;

            AddDomainEvent(new PatientRegisteredEvent(PatientId, Name, Age, Gender, Department, PhoneNumber, Email, Address, BloodGroup, HospitalId, PrimaryDoctorId));
        }

        public void UpdateDetails(string name, int age, string gender, string department,
                                  string phoneNumber, string email, string address, string bloodGroup,Guid hospitalId,Guid doctorId,DateTime updatedAt)
        {
            Name = name;
            Age = age;
            Gender = gender;
            Department = department;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            PrimaryDoctorId = doctorId;
            ModifiedAt = DateTime.UtcNow; // Update the time of update


            AddDomainEvent(new PatientUpdatedEvent(PatientId, Name, Age, Gender, Department, PhoneNumber,Email,Address,BloodGroup,HospitalId, PrimaryDoctorId));
        }

        public void MarkAsDeleted()
        {
            AddDomainEvent(new PatientDeletedEvent(PatientId));
        }
    }
}