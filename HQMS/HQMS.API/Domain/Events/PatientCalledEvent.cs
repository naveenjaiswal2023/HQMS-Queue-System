using HQMS.Domain.Common;
using MediatR;

namespace HQMS.Domain.Events
{
    public class PatientCalledEvent : IDomainEvent, INotification
    {
        public int PatientId { get; }
        public int DoctorId { get; }

        public PatientCalledEvent(int patientId, int doctorId)
        {
            PatientId = patientId;
            DoctorId = doctorId;
        }
    }
}
