using HQMS.API.Domain.Enum;
using HQMS.API.Domain.Events;
using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class QueueItem : BaseEntity
    {
        public Guid Id { get; private set; }

        public Guid DoctorId { get; private set; }
        public Doctor Doctor { get; private set; }

        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; }

        public Guid AppointmentId { get; private set; }
        public Appointment Appointment { get; private set; }

        public Guid DepartmentId { get; private set; }
        public Department Department { get; private set; }

        public int Position { get; private set; }
        public TimeSpan EstimatedWaitTime { get; private set; }

        public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;
        public DateTime? CalledAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }

        public QueueStatus Status { get; private set; } = QueueStatus.Pending;

        public string QueueNumber { get; private set; }

        // ✅ Constructor with Domain Event raised
        public QueueItem(
            Guid id,
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            Guid departmentId,
            int position,
            TimeSpan estimatedWaitTime,
            string queueNumber)
        {
            Id = id;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            DepartmentId = departmentId;
            Position = position;
            EstimatedWaitTime = estimatedWaitTime;
            QueueNumber = queueNumber;
            JoinedAt = DateTime.UtcNow;
            Status = QueueStatus.Pending;

            // ✅ Raise PatientQueuedEvent when created
            AddDomainEvent(new PatientQueuedEvent(
                queueItemId: Id,
                queueNumber: queueNumber,
                doctorId: doctorId,
                patientId: patientId,
                appointmentId: appointmentId,
                joinedAt: JoinedAt
            ));
        }

        // Domain behaviors

        public void SetQueueNumber(string number)
        {
            QueueNumber = number;
        }

        public void MarkAsCalled()
        {
            Status = QueueStatus.Called;
            CalledAt = DateTime.UtcNow;
            AddDomainEvent(new QueueItemCalledEvent(Id));
        }

        public void MarkAsCompleted()
        {
            Status = QueueStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }
    }
}