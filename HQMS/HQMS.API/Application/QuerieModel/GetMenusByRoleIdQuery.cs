using HQMS.API.Domain.Events;
using MediatR;

namespace HQMS.API.Application.QuerieModel
{
    public class GetMenusByRoleIdQuery : IRequest<List<MenuCreatedEvent>>
    {
        public string RoleId { get; }

        public GetMenusByRoleIdQuery(string roleId)
        {
            RoleId = roleId;
        }
    }
}
