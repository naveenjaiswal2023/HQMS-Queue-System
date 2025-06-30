using HQMS.API.Domain.Entities;
using HQMS.Application.Commands;
using HQMS.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.Application.Handlers
{
    public class RegisterPatientCommandHandler : IRequestHandler<RegisterPatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RegisterPatientCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public RegisterPatientCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<RegisterPatientCommandHandler> logger,
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _domainEventPublisher = domainEventPublisher ?? throw new ArgumentNullException(nameof(domainEventPublisher));
        }

        public async Task<bool> Handle(RegisterPatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Name) ||
                    string.IsNullOrWhiteSpace(request.Gender) ||
                    request.DepartmentId == Guid.Empty) // Fix: Check if DepartmentId is an empty Guid instead of using string.IsNullOrWhiteSpace
                {
                    _logger.LogWarning("Invalid patient data received.");
                    return false;
                }

                var patient = new Patient(request.Name, request.Age, request.Gender, request.DepartmentId, request.PhoneNumber, request.Email, request.Address, request.BloodGroup, request.HospitalId, request.DoctorId);
                await _unitOfWork.PatientRepository.AddAsync(patient);
                _unitOfWork.Context.Patients.Add(patient);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Publish domain events
                foreach (var domainEvent in patient.DomainEvents)
                {
                    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                }

                patient.ClearDomainEvents();

                _logger.LogInformation("Patient {PatientId} registered successfully.", patient.PatientId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register patient.");
                return false;
            }
        }
    }

}
