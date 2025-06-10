using HospitalQueueSystem.Domain.Events;
using MediatR;
using Microsoft.Azure.Amqp.Framing;

namespace HospitalQueueSystem.Application.CommandModel
{
    public class UpdatePatientCommand : IRequest<bool>
    {
        public string PatientId { get; }
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

        public DateTime UpdatedAt { get; }= DateTime.UtcNow; // Default to current time

        public UpdatePatientCommand(string patientId, string name, int age, string gender, string department, string phoneNumber, string emailId, string address, string bloodGroup, Guid hospitalId, Guid doctorId,DateTime updatedAt)
        {
            PatientId = patientId;
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
            UpdatedAt = updatedAt; // Capture the time of update
        }

        // Optional constructor overload for convenience if you're passing an event/model
        public UpdatePatientCommand(PatientUpdatedEvent patient)
        {
            PatientId = patient.PatientId.ToString(); // Fix: Convert Guid to string
            Name = patient.Name;
            Age = patient.Age;
            Gender = patient.Gender;
            Department = patient.Department;
            PhoneNumber = patient.PhoneNumber;
            Email = patient.Email;
            Address = patient.Address;
            BloodGroup = patient.BloodGroup;
            HospitalId = patient.HospitalId;
            DoctorId = patient.DoctorId;
            UpdatedAt=patient.UpdatedAt; // Use the UpdatedAt from the event
        }
    }
}
