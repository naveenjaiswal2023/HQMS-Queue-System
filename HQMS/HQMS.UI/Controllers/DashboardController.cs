using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HospitalQueueSystem.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            //var signalRHubUrl = _configuration["SignalR:HubUrl"];
            //ViewBag.SignalRHubUrl = signalRHubUrl;
            return View();
        }
    }
}
