using Microsoft.AspNetCore.SignalR;

namespace HQMS.Infrastructure.SignalR
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string eventName, object message)
        {
            await Clients.All.SendAsync("ReceiveNotification", eventName, message);
        }

    }

}
