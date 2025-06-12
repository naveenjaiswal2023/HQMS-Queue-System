using HQMS.API.Domain.Events;
using MediatR;

namespace HQMS.API.Application.CommandModel
{
    public record CreateMenuCommand : IRequest<bool>
    {
        public string Name { get; init; }
        public string? Url { get; init; }
        public string? Icon { get; init; }
        public int OrderBy { get; init; }
        public string? PermissionKey { get; init; }
        public Guid? ParentMenuId { get; init; }
        public List<Guid>? RoleIds { get; init; }

        public CreateMenuCommand(
            string name,
            string? url,
            string? icon,
            int orderBy,
            string? permissionKey,
            Guid? parentMenuId,
            List<Guid>? roleIds = null)
        {
            Name = name;
            Url = url;
            Icon = icon;
            OrderBy = orderBy;
            PermissionKey = permissionKey;
            ParentMenuId = parentMenuId;
            RoleIds = roleIds;
        }
    }
}
