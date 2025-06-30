using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using HQMS.API.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, List<RoleUpdatedEvent>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;
        private readonly ICacheService _cacheService;

        public GetRoleByIdQueryHandler(
            RoleManager<ApplicationRole> roleManager,
            ILogger<GetRoleByIdQueryHandler> logger,
            ICacheService cacheService)
        {
            _roleManager = roleManager;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<List<RoleUpdatedEvent>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"Role:{request.RoleId}";

            // ✅ Try from cache
            var cachedRole = await _cacheService.GetAsync<List<RoleUpdatedEvent>>(cacheKey);
            if (cachedRole != null)
            {
                _logger.LogInformation("✅ Role found in cache for ID: {RoleId}", request.RoleId);
                return cachedRole;
            }

            try
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId);

                if (role == null)
                {
                    _logger.LogWarning("⚠️ Role with ID {RoleId} not found.", request.RoleId);
                    return null;
                }

                var roleEvent = new RoleUpdatedEvent(role.Id, role.Name);
                var result = new List<RoleUpdatedEvent> { roleEvent };

                // ✅ Store in cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error retrieving role from database.");
                return null;
            }
        }
    }
}
