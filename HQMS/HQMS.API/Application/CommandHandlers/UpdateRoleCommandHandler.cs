
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using HQMS.API.Domain.Entities;
using HQMS.Application.Common; // Assuming ApplicationRole is here

namespace HQMS.API.Application.CommandHandlers
{
    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<UpdateRoleCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public UpdateRoleCommandHandler(
            ILogger<UpdateRoleCommandHandler> logger,
            RoleManager<ApplicationRole> roleManager,
            IDomainEventPublisher domainEventPublisher)
        {
            _roleManager = roleManager;
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
        }

        public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId);

                if (role == null)
                {
                    _logger.LogWarning("Role with ID {RoleId} not found.", request.RoleId);
                    return false;
                }

                // Update the role using domain method
                role.Update(request.NewRoleName);

                var result = await _roleManager.UpdateAsync(role);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Failed to update role: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    return false;
                }

                // Publish domain events
                foreach (var domainEvent in role.DomainEvents)
                {
                    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                }

                role.ClearDomainEvents();

                _logger.LogInformation("Role {RoleId} updated to '{RoleName}' successfully.", role.Id, role.Name);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role with ID {RoleId}", request.RoleId);
                return false;
            }
        }
    }
}
