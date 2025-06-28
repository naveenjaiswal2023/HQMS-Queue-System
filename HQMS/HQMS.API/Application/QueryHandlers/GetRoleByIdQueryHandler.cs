
using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using StackExchange.Redis;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, List<RoleUpdatedEvent>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<GetRoleByIdQueryHandler> _logger;

        public GetRoleByIdQueryHandler(RoleManager<ApplicationRole> roleManager, ILogger<GetRoleByIdQueryHandler> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<RoleUpdatedEvent>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId);

                if (role == null)
                {
                    _logger.LogWarning("Role with ID {RoleId} not found.", request.RoleId);
                    return null;
                }

                var RoleEvent = new RoleUpdatedEvent(
                    role.Id,
                    role.Name
                );

                return new List<RoleUpdatedEvent> { RoleEvent };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving role.");
                return null;
            }

        }
    }
}
