using HospitalQueueSystem.Domain.Common;
using MediatR;

namespace HQMS.API.Domain.Events
{
    public class RoleUpdatedEvent : IDomainEvent
    {
        public string RoleId { get; }
        public string NewRoleName { get; }

        public RoleUpdatedEvent(string roleId, string newRoleName)
        {
            RoleId = roleId;
            NewRoleName = newRoleName;
        }
    }

}
