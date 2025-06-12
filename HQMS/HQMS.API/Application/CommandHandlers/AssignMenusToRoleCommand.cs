using HospitalQueueSystem.Application.Common;
using HospitalQueueSystem.Domain.Common;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace HQMS.API.Application.CommandHandlers
{
    public class AssignMenusToRoleCommandHandler : IRequestHandler<AssignMenusToRoleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignMenusToRoleCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public AssignMenusToRoleCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<AssignMenusToRoleCommandHandler> logger,
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<bool> Handle(AssignMenusToRoleCommand request, CancellationToken cancellationToken)
        {
            // 1. Remove existing RoleMenus
            if (Guid.TryParse(request.RoleId, out var roleIdGuid)) // Convert RoleId from string to Guid
            {
                var existing = await _unitOfWork.RoleMenuRepository.GetByIdAsync(roleIdGuid);
                if (existing != null) // Ensure existing is not null
                {
                    await _unitOfWork.RoleMenuRepository.RemoveRange(new List<RoleMenu> { existing });
                }
            }
            else
            {
                _logger.LogError("Invalid RoleId format: {RoleId}", request.RoleId);
                throw new ArgumentException("RoleId must be a valid GUID.", nameof(request.RoleId));
            }

            // 2. Add new RoleMenus and collect domain events
            List<IDomainEvent> domainEvents = new();

            foreach (var menuId in request.MenuIds)
            {
                var roleMenu = new RoleMenu(request.RoleId, menuId);
                await _unitOfWork.RoleMenuRepository.AddAsync(roleMenu);

                domainEvents.AddRange(roleMenu.DomainEvents); // ✅ from the entity instance
                roleMenu.ClearDomainEvents(); // Optional but clean
            }

            // 3. Save DB changes
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Publish domain events
            foreach (var domainEvent in domainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            _logger.LogInformation("Menus assigned to role {RoleId}", request.RoleId);
            return true;
        }

    }
}
