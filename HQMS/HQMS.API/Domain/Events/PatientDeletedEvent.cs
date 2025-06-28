using HQMS.Domain.Common;
using MediatR;

namespace HQMS.Domain.Events
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
