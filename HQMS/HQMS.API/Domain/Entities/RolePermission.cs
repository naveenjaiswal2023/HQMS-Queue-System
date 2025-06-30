using HospitalQueueSystem.Domain.Common;
using HQMS.API.Domain.Events;
using HQMS.Domain.Entities.Common;

namespace HQMS.API.Domain.Entities
{
    public class RolePermission : BaseEntity
    {
        public string RoleId { get; private set; } // Changed from string to Guid
        public int PermissionId { get; private set; }

        // ✅ Navigation properties
        public virtual ApplicationRole Role { get; private set; }

        public virtual Permission Permission { get; private set; }

        // ✅ Constructor used by your application logic
        public RolePermission(string roleId, int permissionId) // Updated parameter type
        {
            RoleId = roleId;
            PermissionId = permissionId;

            AddDomainEvent(new PermissionAssignedToRoleEvent(roleId, permissionId)); // No changes needed here
        }

        // ✅ Required by EF Core for materialization
        private RolePermission() { }
    }
}
