using HQMS.Domain.Common;
using MediatR;

namespace HQMS.Domain.Events
{
    public class PatientUpdatedEvent : IDomainEvent, INotification
    {
        public Guid PatientId { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public Guid DepartmentId { get; }      // ✅ updated
        public DateTime RegisteredAt { get; }  // ✅ optional: consider removing if unused
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }
        public DateTime UpdatedAt { get; }

        public PatientUpdatedEvent(
            Guid patientId,
            string name,
            int age,
            string gender,
            Guid departmentId,              // ✅ updated
            string phoneNumber,
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
            DepartmentId = departmentId;   // ✅ updated
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
