using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities; // ApplicationRole
using HQMS.API.Domain.Events;
using HospitalQueueSystem.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.CommandHandlers
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IDomainEventPublisher _domainEventPublisher;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(
            RoleManager<ApplicationRole> roleManager,
            IDomainEventPublisher domainEventPublisher,
            ILogger<CreateRoleCommandHandler> logger)
        {
            _roleManager = roleManager;
            _domainEventPublisher = domainEventPublisher;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = ApplicationRole.Create(request.RoleName); // Adds RoleCreatedEvent

                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to create role '{RoleName}': {Errors}", request.RoleName,
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                foreach (var domainEvent in role.DomainEvents)
                {
                    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                }

                role.ClearDomainEvents();
                _logger.LogInformation("Role '{RoleName}' created successfully with ID: {RoleId}", role.Name, role.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating role '{RoleName}'", request.RoleName);
                return false;
            }
        }
    }
}
