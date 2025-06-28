using HQMS.Domain.Common;

namespace HQMS.API.Domain.Events
{
    public record MenusAssignedToRoleEvent : IDomainEvent
    {
        public Guid RoleId { get; init; }
        public List<Guid> MenuIds { get; init; }
        public MenusAssignedToRoleEvent(Guid roleId, List<Guid> menuIds)
        {
            RoleId = roleId;
            MenuIds = menuIds;
        }
    }
}
