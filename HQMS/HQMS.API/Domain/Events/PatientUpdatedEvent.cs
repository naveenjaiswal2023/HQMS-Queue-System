using HospitalQueueSystem.Domain.Common;
using MediatR;
using System.Text.Json.Serialization;

namespace HospitalQueueSystem.Domain.Events
{
    public class PatientUpdatedEvent : IDomainEvent, INotification
    {
        public Guid PatientId { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public string Department { get; }
        public DateTime RegisteredAt { get; }
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }
        public DateTime UpdatedAt { get; }

        //[JsonConstructor]
        public PatientUpdatedEvent(
        Guid patientId,
        string name,
        int age,
        string gender,
        string department,
        string phoneNumber,     // ✅ was `phoneNo`
        string email,
        string address,
        string bloodGroup,
        Guid hospitalId,
        Guid doctorId)
        {
            PatientId = patientId;
            Name = name;
            Age = age;
            Gender = gender;
            Department = department;
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
            UpdatedAt = DateTime.UtcNow; // Set to current time by default
        }

    }
}
