using HospitalQueueSystem.Domain.Events;
using MediatR;

namespace HospitalQueueSystem.Application.Commands
{
    public class RegisterPatientCommand : IRequest<bool>
    {
        public string Name { get; }
        public int Age { get; }
        public string Gender { get; }
        public string Department { get; }
        public DateTime RegisteredAt { get; } = DateTime.UtcNow; // Default to current time
        public string PhoneNumber { get; }
        public string Email { get; }
        public string Address { get; }
        public string BloodGroup { get; }
        public Guid HospitalId { get; }
        public Guid DoctorId { get; }

        public RegisterPatientCommand(string name, int age, string gender, string department, string phoneNumber, string emailId, string address, string bloodGroup, Guid hospitalId, Guid doctorId)
        {
            Name = name;
            Age = age;
            Gender = gender;
            Department = department;
            PhoneNumber = phoneNumber;
            Email = emailId;
            Address = address;
            BloodGroup = bloodGroup;
            HospitalId = hospitalId;
            DoctorId = doctorId;
            HospitalId= hospitalId;
            DoctorId = doctorId;
        }
    }
}
