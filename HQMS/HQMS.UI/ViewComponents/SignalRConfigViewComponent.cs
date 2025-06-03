using HQMS.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HQMS.UI.ViewComponents
{
    public class SignalRConfigViewComponent : ViewComponent
    {
        private readonly SignalRSettings _settings;

        public SignalRConfigViewComponent(IOptions<SignalRSettings> settings)
        {
            _settings = settings.Value;
        }

        public IViewComponentResult Invoke()
        {
            return View("Default", _settings);
        }
    }
}
