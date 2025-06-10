using HospitalQueueSystem.Application.Common;
using HospitalQueueSystem.Application.Handlers;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HQMS.API.Application.CommandHandlers
{
    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<DeleteRoleCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public DeleteRoleCommandHandler(
            RoleManager<ApplicationRole> roleManager,  // Use ApplicationRole here
            ILogger<DeleteRoleCommandHandler> logger,
            IDomainEventPublisher domainEventPublisher,
            IMediator mediator)
        {
            _roleManager = roleManager;
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
        }

        public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.RoleId) as ApplicationRole;

            if (role == null)
            {
                _logger.LogWarning("Role with ID {RoleId} not found.", request.RoleId);
                return false;
            }

            role.AddDomainEvent(new RoleDeletedEvent(role.Id, role.Name));

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                foreach (var domainEvent in role.DomainEvents)
                {
                    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                }

                role.ClearDomainEvents();
                return true;
            }

            _logger.LogError("Failed to delete role {RoleId}", request.RoleId);
            return false;
        }

    }

}
