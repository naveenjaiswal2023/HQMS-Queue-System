using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using HQMS.API.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleCreatedEvent>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;
        private readonly ICacheService _cacheService;

        private const string CacheKey = "AllRoles";

        public GetAllRolesQueryHandler(
            RoleManager<ApplicationRole> roleManager,
            ILogger<GetAllRolesQueryHandler> logger,
            ICacheService cacheService)
        {
            _roleManager = roleManager;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<List<RoleCreatedEvent>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // ✅ Check Cache
                var cachedRoles = await _cacheService.GetAsync<List<RoleCreatedEvent>>(CacheKey);
                if (cachedRoles != null)
                    return cachedRoles;

                var roles = _roleManager.Roles.ToList();

                if (!roles.Any())
                {
                    _logger.LogWarning("No roles found.");
                    return new List<RoleCreatedEvent>();
                }

                var result = roles
                    .Select(r => new RoleCreatedEvent(r.Id, r.Name))
                    .ToList();

                // ✅ Set Cache
                await _cacheService.SetAsync(CacheKey, result, TimeSpan.FromMinutes(10));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving roles.");
                return new List<RoleCreatedEvent>();
            }
        }
    }
}
