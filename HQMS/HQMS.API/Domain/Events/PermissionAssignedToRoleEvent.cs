using HQMS.Domain.Common;

namespace HQMS.API.Domain.Events
{
    public class PermissionAssignedToRoleEvent : IDomainEvent
    {
        public string RoleId { get; }
        public int PermissionId { get; }

        public PermissionAssignedToRoleEvent(string roleId, int permissionId)
        {
            RoleId = roleId;
            PermissionId = permissionId;
        }
    }
}
