using HQMS.API.Application.QuerieModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleCreatedEvent>>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<GetAllRolesQueryHandler> _logger;

        public GetAllRolesQueryHandler(RoleManager<ApplicationRole> roleManager, ILogger<GetAllRolesQueryHandler> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<List<RoleCreatedEvent>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var roles = _roleManager.Roles.ToList();

                if (!roles.Any())
                {
                    _logger.LogWarning("No roles found.");
                    return new List<RoleCreatedEvent>();
                }

                var result = roles.Select(r => new RoleCreatedEvent(r.Id, r.Name)).ToList();

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
