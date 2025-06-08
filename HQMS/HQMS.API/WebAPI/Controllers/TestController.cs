using HospitalQueueSystem.Infrastructure.SignalR;
using HQMS.API.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HQMS.API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        public TestController(IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        [HttpPost("send-test-notification")]
        public async Task<IActionResult> SendTestNotification()
        {
            var testMessage = new { name = "Test Patient from Background Service" };
            await _notificationService.SendNotificationAsync("PatientRegisteredEvent", testMessage);
            //await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            //    "PatientRegisteredEvent",
            //    new { name = "Test Patient" });

            return Ok("Notification sent");
        }
    }
}
