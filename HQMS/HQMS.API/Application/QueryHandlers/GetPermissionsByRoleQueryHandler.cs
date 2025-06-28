
using HQMS.API.Application.DTO;
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetPermissionsByRoleQueryHandler : IRequestHandler<GetPermissionsByRoleQuery, RolePermissionViewModel>
    {
        private readonly ILogger<GetPermissionsByRoleQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public GetPermissionsByRoleQueryHandler(ILogger<GetPermissionsByRoleQueryHandler> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<RolePermissionViewModel> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
        {
            // Validate input RoleId
            if (string.IsNullOrEmpty(request.RoleId.ToString()))
            {
                _logger.LogWarning("RoleId is null or empty.");
                return null;
            }

            // Get the role with its details
            var role = await _unitOfWork.RolePermissionRepository.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found for ID: {RoleId}", request.RoleId);
                return null;
            }

            // Get RolePermissions with related Permission and Menu
            var rolePermissions = await _unitOfWork.RolePermissionRepository
                .GetRolePermissionsWithMenuAndPermissionAsync(request.RoleId);

            var permissions = rolePermissions
                .Where(rp => rp.Permission != null) // Ensure Permission is not null
                .Select(rp => new PermissionWithMenu
                {
                    PermissionId = rp.Permission.Id,
                    PermissionName = rp.Permission.Name,
                    Menu = rp.Permission.Menu != null
                        ? new MenuDto
                        {
                            MenuId = rp.Permission.Menu.MenuId.ToString(),
                            Name = rp.Permission.Menu.Name,
                            Url= rp.Permission.Menu.Url,
                            Icon = rp.Permission.Menu.Icon,
                            OrderBy = rp.Permission.Menu.OrderBy,
                            ParentMenuId = rp.Permission.Menu.ParentId.HasValue ? rp.Permission.Menu.ParentId.Value : (Guid?)null,
                        }
                        : null // Or use a default MenuDto if you want
                }).ToList();

            return new RolePermissionViewModel
            {
                RoleId = role.RoleId.ToString(),
                RoleName = role.Role?.Name ?? "Unknown Role",
                Permissions = permissions
            };
        }
    }
}
