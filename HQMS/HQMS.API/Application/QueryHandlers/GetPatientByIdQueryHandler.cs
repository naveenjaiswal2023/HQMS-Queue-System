using HospitalQueueSystem.Application.Queries;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.QuerieModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, List<PatientRegisteredEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly ILogger<GetPatientByIdQueryHandler> _logger;

        public GetPatientByIdQueryHandler(IUnitOfWork unitOfWork, IDistributedCache cache, ILogger<GetPatientByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<PatientRegisteredEvent>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);

                if (patient == null)
                {
                    _logger.LogWarning($"Patient with ID {request.PatientId} not found.");
                    return new List<PatientRegisteredEvent>();
                }

                var patientEvent = new PatientRegisteredEvent(
                    patient.PatientId,
                    patient.Name,
                    patient.Age,
                    patient.Gender,
                    patient.Department,
                    patient.RegisteredAt
                );

                return new List<PatientRegisteredEvent> { patientEvent };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving patient with ID {request.PatientId}.");
                return new List<PatientRegisteredEvent>();
            }
        }
    }
}
