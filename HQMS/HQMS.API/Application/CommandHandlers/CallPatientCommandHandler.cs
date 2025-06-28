using HQMS.Application.CommandModel;
using HQMS.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.Application.CommandHandlers
{
    public class CallPatientCommandHandler : IRequestHandler<CallPatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<CallPatientCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public CallPatientCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<CallPatientCommandHandler> logger, 
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
        }

        public async Task<bool> Handle(CallPatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Convert QueueEntryId to string before passing to GetByIdAsync
                var entry = await _unitOfWork.QueueRepository.GetByIdAsync(request.QueueEntryId);
                if (entry == null)
                {
                    _logger.LogWarning("Queue entry not found for ID: {QueueEntryId}", request.QueueEntryId);
                    return false;
                }

                // Update status using the appropriate method
                entry.MarkAsCalled();

                // Persist changes
                await _unitOfWork.QueueRepository.UpdateAsync(entry);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Fix: Use Guid.ToString() to convert PatientId and DoctorId to strings
                var patientId = entry.PatientId.ToString();
                var doctorId = entry.DoctorId.ToString();

                // Publish domain events
                //foreach (var domainEvent in entry.DomainEvents)
                //{
                //    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                //}

                //entry.ClearDomainEvents();
                _logger.LogInformation("Patient {PatientId} Called successfully.", entry.PatientId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calling patient for QueueEntryId: {QueueEntryId}", request.QueueEntryId);
                return false;
            }
        }
    }
}
