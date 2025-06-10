using HospitalQueueSystem.Domain.Common;
using MediatR;

namespace HospitalQueueSystem.Domain.Events
{
    public class PatientDeletedEvent : IDomainEvent, INotification
    {
        public Guid PatientId { get; }

        public PatientDeletedEvent(Guid patientId)
        {
            PatientId = patientId;
        }
    }
}
