
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.CommandModel
{
    public class DeleteRoleCommand : IRequest<bool>
    {
        public string RoleId { get; set; }
        public DeleteRoleCommand(string roleId)
        {
            RoleId = roleId;
        }
    }

}
