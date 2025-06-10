using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace HQMS.API.Application.QuerieModel
{
    public class GetRoleByIdQuery : IRequest<List<RoleUpdatedEvent>>
    {
        public string RoleId { get; }

        public GetRoleByIdQuery(string roleId)
        {
            RoleId = roleId;
        }

    }

}
