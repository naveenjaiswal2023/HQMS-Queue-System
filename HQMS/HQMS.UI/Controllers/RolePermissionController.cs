using HQMS.UI.Interfaces;
using HQMS.UI.Models;
using HQMS.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.UI.Controllers
{
    public class RolePermissionController : Controller
    {
        private readonly IRolePermissionService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;

        public RolePermissionController(IRolePermissionService service, IHttpContextAccessor httpContextAccessor, IRoleService roleService)
        {
            _service = service;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> AssignPermissions(Guid? roleId)
        {
            Guid selectedRoleId;

            if (roleId.HasValue && roleId != Guid.Empty)
            {
                selectedRoleId = roleId.Value;
                HttpContext.Session.SetString("RoleId", selectedRoleId.ToString());
            }
            else
            {
                var sessionRoleId = _httpContextAccessor.HttpContext?.Session?.GetString("RoleId");
                if (string.IsNullOrEmpty(sessionRoleId) || !Guid.TryParse(sessionRoleId, out selectedRoleId))
                {
                    return View(new RolePermissionAssignmentViewModel
                    {
                        AvailableRoles = await GetAllRolesAsync()
                    });
                }
            }

            var data = await _service.GetPermissionsByRoleAsync(selectedRoleId);
            var menuGroups = data?.Permissions?
                .GroupBy(p => p.Menu.menuId)
                .Select(g => new MenuPermissionOption
                {
                    MenuId = Guid.Parse(g.Key),
                    MenuName = g.First().Menu.Name,
                    Permissions = g.Select(p => new PermissionCheckbox
                    {
                        PermissionId = p.permissionId,
                        PermissionName = p.permissionName,
                        IsAssigned = true
                    }).ToList()
                }).ToList() ?? new List<MenuPermissionOption>();

            var model = new RolePermissionAssignmentViewModel
            {
                RoleId = selectedRoleId.ToString(),
                RoleName = data.roleName,
                MenuPermissions = menuGroups,
                AvailableRoles = await GetAllRolesAsync(selectedRoleId)
            };

            return View(model);
        }

        private async Task<List<SelectListItem>> GetAllRolesAsync(Guid? selectedRoleId = null)
        {
            var roles = await _roleService.GetRolesAsync();

            foreach (var role in roles)
            {
                role.Selected = selectedRoleId.HasValue && role.Value == selectedRoleId.ToString();
            }

            return roles;
        }

        [HttpPost]
        public async Task<IActionResult> AssignPermissions(RolePermissionAssignmentViewModel model)
        {
            var assignedPermissionIds = model.MenuPermissions
                .SelectMany(m => m.Permissions)
                .Where(p => p.IsAssigned)
                .Select(p => p.PermissionId)
                .ToList();

            // Call API to assign permissions
            await _service.AssignPermissionsToRole(model.RoleId, assignedPermissionIds);

            return RedirectToAction("AssignPermissions", new { roleId = model.RoleId });
        }   
    }

}
