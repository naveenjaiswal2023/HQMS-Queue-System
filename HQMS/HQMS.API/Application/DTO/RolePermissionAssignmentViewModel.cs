using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.API.Application.DTO
{
    public class RolePermissionAssignmentViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuPermissionGroup> MenuPermissions { get; set; } = new();
        public List<SelectListItem> AvailableRoles { get; set; } = new();
    }
}
