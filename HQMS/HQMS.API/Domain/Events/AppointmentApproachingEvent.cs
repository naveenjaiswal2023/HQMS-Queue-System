using HQMS.Domain.Common;
using HQMS.API.Domain.Entities;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class AppointmentApproachingEvent : IDomainEvent, INotification
    {
        public Appointment Appointment { get; }

        public AppointmentApproachingEvent(Appointment appointment)
        {
            Appointment = appointment;
        }
    }
}
