using HospitalQueueSystem.Application.Common;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.CommandHandlers
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CreateMenuCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public CreateMenuCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<CreateMenuCommandHandler> logger,
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<bool> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            // Create Menu aggregate
            var menu = new Menu(
                name: request.Name,
                url: request.Url,
                icon: request.Icon,
                orderBy: request.OrderBy,
                permissionKey: request.PermissionKey,
                parentId: request.ParentMenuId
            );

            // Assign roles if any
            if (request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    menu.AddRole(roleId.ToString()); // Your domain method to attach RoleMenu
                }
            }

            // Persist Menu
            await _unitOfWork.MenuRepository.AddAsync(menu);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Publish Domain Events
            foreach (var domainEvent in menu.DomainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            menu.ClearDomainEvents();

            _logger.LogInformation("✅ Menu '{MenuName}' created with ID {MenuId}.", menu.Name, menu.MenuId);
            return true;
        }
    }
}
