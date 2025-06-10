using MediatR;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

namespace HQMS.API.Application.CommandModel
{
    public class CreateRoleCommand: IRequest<bool>
    {
        public string RoleName { get; }
        public CreateRoleCommand(string roleName)
        {
            RoleName = roleName;

        }
    }
}
