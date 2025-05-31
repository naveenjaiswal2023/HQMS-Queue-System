using Microsoft.AspNetCore.Mvc;

namespace HospitalQueueSystem.Web.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
