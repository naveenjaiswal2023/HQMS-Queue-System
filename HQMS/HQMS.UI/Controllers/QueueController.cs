using HQMS.UI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HQMS.UI.Controllers
{
    [Authorize(Roles = "POD,Doctor")]
    public class QueueController : Controller
    {
        private readonly IQueueService _dashboardService;

        public QueueController(IQueueService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(Guid? hospitalId, Guid? departmentId, Guid? doctorId)
        {
            var data = await _dashboardService.GetDashboardAsync(hospitalId, departmentId, doctorId);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CallQueue(string queueNumber)
        {
            // Publish a SignalR or event to notify display boards or doctors
            // Example: await _eventPublisher.PublishAsync(new PatientCalledEvent(queueNumber));

            TempData["Success"] = $"Queue {queueNumber} called successfully!";
            return RedirectToAction("Index");
        }

    }

}
