using HQMS.API.Domain.Entities;

namespace HQMS.API.Domain.Interfaces
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<List<RolePermission>> GetRolePermissionsWithMenuAndPermissionAsync(Guid roleId);
        Task<List<RolePermission>> GetByRoleIdAsync(Guid roleId);
    }
}
