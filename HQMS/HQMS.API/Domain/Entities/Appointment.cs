
using HQMS.API.Domain.Events;
using HQMS.Domain.Common;
using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid Id { get; private set; }
        public Guid HospitalId { get; private set; }
        public Guid DoctorId { get; private set; }
        public Guid PatientId { get; private set; }
        public DateTime AppointmentTime { get; private set; }

        public bool QueueGenerated { get; private set; } = false;

        // Constructor
        public Appointment(Guid id, Guid hospitalId, Guid doctorId, Guid patientId, DateTime appointmentTime)
        {
            Id = id;
            HospitalId = hospitalId;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentTime = appointmentTime;
        }

        // Business Logic
        public void RaiseQueueTriggerEventIfDue(DateTime currentTime, Guid queueItemId,string queueNumber, string doctorName, string patientName)
        {
            if (!QueueGenerated && AppointmentTime.AddMinutes(-15) <= currentTime)
            {
                AddDomainEvent(new PatientQueuedEvent(
                    queueItemId,
                    queueNumber,
                    DoctorId,
                    PatientId,
                    Id,
                    DateTime.UtcNow));
            }
        }

        public void MarkQueueGenerated()
        {
            QueueGenerated = true;
        }

        public void UpdateAppointmentDetails(Guid doctorId, Guid patientId, DateTime appointmentTime)
        {
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentTime = appointmentTime;
        }

        // Navigation Properties
        public Patient Patient { get; private set; } = null!;
        public Doctor Doctor { get; private set; } = null!;
        // public Hospital Hospital { get; private set; }  // uncomment if needed
    }
}
