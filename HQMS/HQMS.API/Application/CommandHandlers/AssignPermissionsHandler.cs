
using HQMS.API.Application.CommandModel;
using HQMS.API.Domain.Entities;
using HQMS.API.Domain.Interfaces;
using HQMS.Application.Common;
using HQMS.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.CommandHandlers
{
    public class AssignPermissionsHandler : IRequestHandler<AssignPermissionsCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AssignPermissionsHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public AssignPermissionsHandler(
            IUnitOfWork unitOfWork,
            ILogger<AssignPermissionsHandler> logger,
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<bool> Handle(AssignPermissionsCommand request, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(request.RoleId, out var roleId))
                throw new ArgumentException($"Invalid RoleId format: {request.RoleId}", nameof(request.RoleId));

            // Step 1: Remove existing permissions
            var existingPermissions = await _unitOfWork.RolePermissionRepository.GetByRoleIdAsync(Guid.Parse(request.RoleId));
            if (existingPermissions.Any())
            {
                _unitOfWork.RolePermissionRepository.RemoveRange(existingPermissions);
                await _unitOfWork.SaveChangesAsync(cancellationToken); // ❗Must save to clear tracker
            }

            // Step 2: Add new permissions
            var domainEvents = new List<IDomainEvent>();

            foreach (var permissionId in request.PermissionIds.Distinct()) // Avoid duplicates
            {
                var rolePermission = new RolePermission(request.RoleId, permissionId); // constructor handles mapping
                await _unitOfWork.RolePermissionRepository.AddAsync(rolePermission);
                domainEvents.AddRange(rolePermission.DomainEvents);
                rolePermission.ClearDomainEvents();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (var domainEvent in domainEvents)
            {
                await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
            }

            _logger.LogInformation("Permissions assigned to role {RoleId}", request.RoleId);
            return true;
        }

    }
}
