using Microsoft.AspNetCore.Mvc.Rendering;

namespace HQMS.UI.Models
{
    public class RolePermissionViewModel
    {
        public string roleId { get; set; }
        public string roleName { get; set; }
        public List<PermissionWithMenu> Permissions { get; set; }
        public List<SelectListItem> AvailableRoles { get; set; } = new(); // For dropdown
    }
}
