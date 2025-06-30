using HQMS.API.Application.DTO;
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetPermissionsByRoleQueryHandler : IRequestHandler<GetPermissionsByRoleQuery, RolePermissionViewModel>
    {
        private readonly ILogger<GetPermissionsByRoleQueryHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public GetPermissionsByRoleQueryHandler(
            ILogger<GetPermissionsByRoleQueryHandler> logger,
            IUnitOfWork unitOfWork,
            ICacheService cacheService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<RolePermissionViewModel> Handle(GetPermissionsByRoleQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RoleId.ToString()))
            {
                _logger.LogWarning("RoleId is null or empty.");
                return null;
            }

            var cacheKey = $"PermissionsByRole:{request.RoleId}";

            // ✅ Check cache first
            var cached = await _cacheService.GetAsync<RolePermissionViewModel>(cacheKey);
            if (cached != null)
            {
                _logger.LogInformation("Permissions loaded from cache for RoleId: {RoleId}", request.RoleId);
                return cached;
            }

            // ❌ Cache miss — proceed to DB
            var role = await _unitOfWork.RolePermissionRepository.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found for ID: {RoleId}", request.RoleId);
                return null;
            }

            var rolePermissions = await _unitOfWork.RolePermissionRepository
                .GetRolePermissionsWithMenuAndPermissionAsync(request.RoleId);

            var permissions = rolePermissions
                .Where(rp => rp.Permission != null)
                .Select(rp => new PermissionWithMenu
                {
                    PermissionId = rp.Permission.Id,
                    PermissionName = rp.Permission.Name,
                    Menu = rp.Permission.Menu != null
                        ? new MenuDto
                        {
                            MenuId = rp.Permission.Menu.MenuId.ToString(),
                            Name = rp.Permission.Menu.Name,
                            Url = rp.Permission.Menu.Url,
                            Icon = rp.Permission.Menu.Icon,
                            OrderBy = rp.Permission.Menu.OrderBy,
                            ParentMenuId = rp.Permission.Menu.ParentId
                        }
                        : null
                }).ToList();

            var result = new RolePermissionViewModel
            {
                RoleId = role.RoleId.ToString(),
                RoleName = role.Role?.Name ?? "Unknown Role",
                Permissions = permissions
            };

            // ✅ Set cache for 30 minutes
            await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));

            return result;
        }
    }
}
