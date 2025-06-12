using HospitalQueueSystem.Web.Models;
using HQMS.UI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HospitalQueueSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMenuService _menuService;

        public HomeController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        public async Task<IActionResult> Index()
        {
            var menus = await _menuService.GetMenuHierarchyAsync();
            ViewBag.Menus = menus;
            return View();
        }
    }
}
