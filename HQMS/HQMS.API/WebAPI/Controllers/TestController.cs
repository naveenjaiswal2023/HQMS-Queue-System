using HQMS.API.Domain.Interfaces;
using HQMS.Infrastructure.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HQMS.API.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public TestController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpPost("send-test-notification")]
        public async Task<IActionResult> SendTestNotification()
        {
            // This payload should mimic real PatientQueuedEvent for testing SignalR
            var testMessage = new
            {
                queueNumber = "Q-101",
                joinedAt = DateTime.Now,
                doctorId = Guid.NewGuid(), // Optional if you want to simulate doctor group
                patientName = "Test Patient"
            };

            await _notificationService.SendNotificationAsync("PatientQueuedEvent", testMessage);

            return Ok("✅ Test notification sent");
        }
    }
}
