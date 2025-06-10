using HospitalQueueSystem.Domain.Common;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class RoleDeletedEvent : IDomainEvent
    {
        public string RoleId { get; }
        public string RoleName { get; }

        public RoleDeletedEvent(string roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }
    }

}
