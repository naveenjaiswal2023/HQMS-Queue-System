
using HQMS.Application.CommandModel;
using HQMS.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalQueueSystem.Application.Handlers
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePatientCommandHandler> _logger;
        private readonly IDomainEventPublisher _domainEventPublisher;

        public UpdatePatientCommandHandler(
            IUnitOfWork unitOfWork,
            ILogger<UpdatePatientCommandHandler> logger,
            IDomainEventPublisher domainEventPublisher)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _domainEventPublisher = domainEventPublisher;
        }

        public async Task<bool> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = await _unitOfWork.Context.Patients
                    .FirstOrDefaultAsync(p => p.PatientId == Guid.Parse(request.PatientId), cancellationToken);

                if (patient == null)
                {
                    _logger.LogWarning("Patient not found.");
                    return false;
                }

                // Convert UpdatedAt from string to DateTime
                //if (!DateTime.TryParse(request.UpdatedAt, out var updatedAt))
                //{
                //    _logger.LogWarning("Invalid UpdatedAt value: {UpdatedAt}", request.UpdatedAt);
                //    return false;
                //}
                // string phoneNumber, string email, string address, string bloodGroup
                patient.UpdateDetails(request.Name, request.Age, request.Gender, request.DepartmentId,request.PhoneNumber,request.Email,request.Address,request.BloodGroup,request.HospitalId,request.DoctorId,request.UpdatedAt);
                var updateCount = await _unitOfWork.PatientRepository.UpdateAsync(patient);
                if (updateCount == 0)
                {
                    _logger.LogWarning("Update failed: No rows affected for Patient ID {PatientId}", request.PatientId);
                    return false;
                }

                foreach (var domainEvent in patient.DomainEvents)
                {
                    await _domainEventPublisher.PublishAsync(domainEvent, cancellationToken);
                }

                patient.ClearDomainEvents();
                _logger.LogInformation("Patient {PatientId} updated successfully.", patient.PatientId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update patient.");
                return false;
            }
        }
    }
}