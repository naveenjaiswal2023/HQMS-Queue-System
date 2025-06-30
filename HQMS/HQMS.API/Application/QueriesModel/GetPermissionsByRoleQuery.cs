using HQMS.API.Application.DTO;
using MediatR;

namespace HQMS.API.Application.QuerieModel
{
    public class GetPermissionsByRoleQuery : IRequest<RolePermissionViewModel>
    {
        public Guid RoleId { get; set; }

        public GetPermissionsByRoleQuery(Guid roleId)
        {
            RoleId = roleId;
        }
    }
}
