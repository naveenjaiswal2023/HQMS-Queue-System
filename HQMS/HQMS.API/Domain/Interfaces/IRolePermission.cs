using HQMS.API.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IRolePermission :IRepository<RolePermission>
    {
        Task<List<int>> GetPermissionIdsByRoleAsync(string roleId);
        Task AssignPermissionsToRoleAsync(string roleId, List<int> permissionIds);
    }
}
