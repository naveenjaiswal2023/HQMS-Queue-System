using HQMS.Domain.Common;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class PatientQueuedEvent : IDomainEvent, INotification
    {
        public Guid QueueItemId { get; }
        public string QueueNumber { get; }
        public Guid DoctorId { get; }
        public Guid PatientId { get; }
        public Guid AppointmentId { get; }
        public DateTime JoinedAt { get; }

        public PatientQueuedEvent(
            Guid queueItemId,
            string queueNumber,
            Guid doctorId,
            Guid patientId,
            Guid appointmentId,
            DateTime joinedAt)
        {
            QueueItemId = queueItemId;
            QueueNumber = queueNumber;
            DoctorId = doctorId;
            PatientId = patientId;
            AppointmentId = appointmentId;
            JoinedAt = joinedAt;
        }
    }
}
