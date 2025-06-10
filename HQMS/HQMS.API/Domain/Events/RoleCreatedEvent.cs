using HospitalQueueSystem.Domain.Common;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class RoleCreatedEvent : IDomainEvent
    {
        public string RoleId { get; }
        public string RoleName { get; }

        public RoleCreatedEvent(string roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }
    }

}
