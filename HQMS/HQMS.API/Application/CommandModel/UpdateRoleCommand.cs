
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.CommandModel
{
    public class UpdateRoleCommand : IRequest<bool>
    {
        public string RoleId { get; set; }
        public string NewRoleName { get; set; }

        public UpdateRoleCommand(string roleId, string newRoleName)
        {
            RoleId = roleId;
            NewRoleName = newRoleName;
        }
        public UpdateRoleCommand(RoleUpdatedEvent role)
        {
            RoleId= role.RoleId;
            NewRoleName = role.NewRoleName;
        }
    }

}
