
using HQMS.API.Domain.Events;
using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.SignalR;
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

        //public async Task SendNotificationAsync(string method, string eventName, object message)
        //{
        //    try
        //    {
        //        _logger.LogInformation("Sending SignalR notification - Method: {Method}, Event: {EventName}, Message: {@Message}",
        //            method, eventName, message);

        //        // This should match exactly what your JavaScript client expects
        //        await _hubContext.Clients.All.SendAsync(method, eventName, message);

        //        _logger.LogInformation("✅ SignalR notification sent successfully");
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "❌ Failed to send SignalR notification");
        //        throw;
        //    }
        //}

        public async Task SendNotificationAsync(string methodName, string eventName, object data)
        {
            try
            {
                // Always notify Admin
                await _hubContext.Clients.Group("ADMIN").SendAsync(methodName, eventName, data);

                // Notify POD users
                await _hubContext.Clients.Group("POD").SendAsync(methodName, eventName, data);

                // Notify Nursing users
                await _hubContext.Clients.Group("NURSING").SendAsync(methodName, eventName, data);

                // Notify specific doctor if applicable
                if (data is PatientQueuedEvent evt && evt.DoctorId != Guid.Empty)
                {
                    await _hubContext.Clients.Group($"DOCTOR_{evt.DoctorId}").SendAsync(methodName, eventName, data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SignalR message for event: {EventName}", eventName);
            }
        }


        // Alternative overload that matches your JavaScript client exactly
        public async Task SendNotificationAsync(string eventName, object message)
        {
            await SendNotificationAsync("ReceiveNotification", eventName, message);
        }
    }

}
