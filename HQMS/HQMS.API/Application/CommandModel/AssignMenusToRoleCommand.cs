using HQMS.API.Domain.Events;
using MediatR;

namespace HQMS.API.Application.CommandModel
{
    public record AssignMenusToRoleCommand : IRequest<bool>
    {    
        public string RoleId { get; }
        public IEnumerable<Guid> MenuIds { get; }
        public AssignMenusToRoleCommand(string roleId, IEnumerable<Guid> menuIds)
        {
            RoleId = roleId;
            MenuIds = menuIds;
        }
    }
}