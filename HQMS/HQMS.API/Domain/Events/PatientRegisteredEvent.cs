using HospitalQueueSystem.Domain.Common;
using HQMS.API.Domain.Entities;
using MediatR;

namespace HospitalQueueSystem.Domain.Events
{
    public class PatientRegisteredEvent : IDomainEvent, INotification
    {
        public Guid PatientId { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public string Department { get; }
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }

        // All parameter names exactly match the property names
        public PatientRegisteredEvent(
            Guid patientId,
            string name,
            int age,
            string gender,
            string department,
            string phoneNumber,              // changed from phoneNo
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
            PhoneNumber = phoneNumber;      // matched
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
        }
    }
}
