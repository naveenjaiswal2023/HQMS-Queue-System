
using HQMS.Domain.Events;
using MediatR;

namespace HQMS.Application.CommandModel
{
    public class UpdatePatientCommand : IRequest<bool>
    {
        public string PatientId { get; }
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }

        public Guid DepartmentId { get; } // ✅ Updated

        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }

        public DateTime UpdatedAt { get; } = DateTime.UtcNow;

        public UpdatePatientCommand(
            string patientId,
            string name,
            int age,
            string gender,
            Guid departmentId,                  // ✅ Updated
            string phoneNumber,
            string emailId,
            string address,
            string bloodGroup,
            Guid hospitalId,
            Guid doctorId,
            DateTime updatedAt)
        {
            PatientId = patientId;
            Name = name;
            Age = age;
            Gender = gender;
            DepartmentId = departmentId;        // ✅ Updated
            PhoneNumber = phoneNumber;
            Email = emailId;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
            UpdatedAt = updatedAt;
        }

        // ✅ Constructor from PatientUpdatedEvent
        public UpdatePatientCommand(PatientUpdatedEvent patient)
        {
            PatientId = patient.PatientId.ToString();
            Name = patient.Name;
            Age = patient.Age;
            Gender = patient.Gender;
            DepartmentId = patient.DepartmentId; // ✅ Updated
            PhoneNumber = patient.PhoneNumber;
            Email = patient.Email;
            Address = patient.Address;
            BloodGroup = patient.BloodGroup;
            HospitalId = patient.HospitalId;
            DoctorId = patient.DoctorId;
            UpdatedAt = patient.UpdatedAt;
        }
    }
}
