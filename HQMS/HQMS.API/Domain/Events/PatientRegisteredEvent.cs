using HQMS.Domain.Common;
using HQMS.API.Domain.Entities;
using MediatR;

namespace HQMS.Domain.Events
{
    public class PatientRegisteredEvent : IDomainEvent, INotification
    {
        public Guid PatientId { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public string DepartmentName { get; } // ✅ changed
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public string HospitalName { get; }
        public string DoctorName { get; }

        public PatientRegisteredEvent(
            Guid patientId,
            string name,
            int age,
            string gender,
            string departmentName,            // ✅ changed
            string phoneNumber,
            string email,
            string address,
            string bloodGroup,
            string hospitalName,
            string doctorName)
        {
            PatientId = patientId;
            Name = name;
            Age = age;
            Gender = gender;
            DepartmentName = departmentName;  // ✅ matched
            PhoneNumber = phoneNumber;
            Email = email;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalName = hospitalName;
            DoctorName = doctorName;
        }
    }
}
