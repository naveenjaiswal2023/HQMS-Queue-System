using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Web.WebPages.Html;

namespace HQMS.UI.Controllers
{
    public class UserController : Controller
    {
        private readonly IRoleService _roleService;

        public UserController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new UserCreateViewModel
            {
                Roles = await _roleService.GetRolesAsync()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await _roleService.GetRolesAsync(); // reload for view
                return View(model);
            }

            // Save logic goes here using model.SelectedRoleId etc.
            return RedirectToAction("Index");
        }
    }

}
