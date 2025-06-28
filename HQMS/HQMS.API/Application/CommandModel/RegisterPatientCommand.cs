using MediatR;

namespace HQMS.Application.Commands
{
    public class RegisterPatientCommand : IRequest<bool>
    {
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public Guid DepartmentId { get; } // ✅ Updated
        public DateTime RegisteredAt { get; } = DateTime.UtcNow;
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }

        public RegisterPatientCommand(
            string name,
            int age,
            string gender,
            Guid departmentId,           // ✅ Updated
            string phoneNumber,
            string emailId,
            string address,
            string bloodGroup,
            Guid hospitalId,
            Guid doctorId)
        {
            Name = name;
            Age = age;
            Gender = gender;
            DepartmentId = departmentId;  // ✅ Updated
            PhoneNumber = phoneNumber;
            Email = emailId;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
        }
    }
}
