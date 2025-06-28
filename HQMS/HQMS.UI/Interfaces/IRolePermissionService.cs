using HQMS.UI.Models;

namespace HQMS.UI.Interfaces
{
    public interface IRolePermissionService
    {
        Task<RolePermissionViewModel> GetPermissionsByRoleAsync(Guid roleId);
        Task AssignPermissionsToRole(string roleId, List<int> permissionIds);
    }
}
