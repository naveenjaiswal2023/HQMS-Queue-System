using HospitalQueueSystem.Infrastructure.SignalR;
using HQMS.API.Domain.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;

namespace HQMS.API.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IHubContext<NotificationHub> hubContext, ILogger<NotificationService> logger)
        {
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task SendNotificationAsync(string method, string eventName, object message)
        {
            try
            {
                _logger.LogInformation("Sending SignalR notification - Method: {Method}, Event: {EventName}, Message: {@Message}",
                    method, eventName, message);

                // This should match exactly what your JavaScript client expects
                await _hubContext.Clients.All.SendAsync(method, eventName, message);

                _logger.LogInformation("✅ SignalR notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send SignalR notification");
                throw;
            }
        }

        // Alternative overload that matches your JavaScript client exactly
        public async Task SendNotificationAsync(string eventName, object message)
        {
            await SendNotificationAsync("ReceiveNotification", eventName, message);
        }
    }

}
