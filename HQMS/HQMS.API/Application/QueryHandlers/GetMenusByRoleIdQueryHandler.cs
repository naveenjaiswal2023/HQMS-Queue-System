
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetMenusByRoleIdQueryHandler : IRequestHandler<GetMenusByRoleIdQuery, List<MenuCreatedEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private readonly ILogger<GetMenusByRoleIdQueryHandler> _logger;

        public GetMenusByRoleIdQueryHandler(
            IUnitOfWork unitOfWork,
            IMemoryCache cache,
            ILogger<GetMenusByRoleIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<MenuCreatedEvent>> Handle(GetMenusByRoleIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!Guid.TryParse(request.RoleId, out var roleId))
                {
                    _logger.LogWarning("Invalid RoleId format: {RoleId}.", request.RoleId);
                    return new();
                }

                string cacheKey = $"menus_{roleId}";

                // ✅ Check cache first
                if (_cache.TryGetValue(cacheKey, out List<MenuCreatedEvent> cachedMenus))
                {
                    _logger.LogInformation("Menus loaded from cache for RoleId {RoleId}", request.RoleId);
                    return cachedMenus;
                }

                // 🔍 Get RoleMenus (many-to-many)
                var roleMenus = await _unitOfWork.RoleMenuRepository.GetByRoleIdAsync(roleId);
                if (roleMenus == null || !roleMenus.Any())
                {
                    _logger.LogWarning("No RoleMenu entries found for RoleId {RoleId}.", request.RoleId);
                    return new();
                }

                // 🎯 Get full menu details
                var menuIds = roleMenus.Select(rm => rm.MenuId).Distinct().ToList();
                var allMenus = await _unitOfWork.MenuRepository.GetAllAsync();

                var matchedMenus = allMenus
                .Where(menu => menuIds.Contains(menu.MenuId))
                .Select(menu => new MenuCreatedEvent(
                    menu.MenuId,
                    menu.Name,
                    menu.Url,
                    menu.Icon,
                    menu.OrderBy,
                    menu.IsActive,
                    menu.IsVisible,
                    menu.PermissionKey,
                    menu.ParentId))
                .ToList();


                // ✅ Store in cache
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10))     // expires 10 mins after last access
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));      // expires 1 hour after created

                _cache.Set(cacheKey, matchedMenus, cacheEntryOptions);
                _logger.LogInformation("Menus cached for RoleId {RoleId}", request.RoleId);

                return matchedMenus;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving menus for RoleId {RoleId}.", request.RoleId);
                return new();
            }
        }
    }
}
