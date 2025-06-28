using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.UI.Models
{
    public class RolePermissionAssignmentViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<MenuPermissionOption> MenuPermissions { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; } = new(); // For dropdown
    }
}
