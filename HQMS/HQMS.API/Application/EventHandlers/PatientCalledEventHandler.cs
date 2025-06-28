using HQMS.Domain.Events;
using HQMS.Infrastructure.SignalR;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace HospitalQueueSystem.Application.EventHandlers
{
    public class PatientCalledEventHandler : INotificationHandler<PatientCalledEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public PatientCalledEventHandler(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PatientCalledEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendAsync("PatientCalled", notification.PatientId, notification.DoctorId);
        }
    }

}
