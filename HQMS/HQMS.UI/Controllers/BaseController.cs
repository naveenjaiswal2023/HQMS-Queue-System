using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HospitalQueueSystem.Web.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var isLoggedIn = HttpContext.Session.GetString("UserName");
            if (string.IsNullOrEmpty(isLoggedIn) && !context.ActionDescriptor.DisplayName.Contains("Auth"))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
            }

            base.OnActionExecuting(context);
        }
    }
}
