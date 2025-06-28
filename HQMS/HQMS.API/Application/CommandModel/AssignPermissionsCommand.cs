using MediatR;

namespace HQMS.API.Application.CommandModel
{
    public class AssignPermissionsCommand : IRequest<bool>
    {
        public string RoleId { get; set; }
        public List<int> PermissionIds { get; set; }
    }
}
