using HospitalQueueSystem.Application.Queries;
using HospitalQueueSystem.Domain.Entities;
using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Domain.Interfaces;
using HQMS.API.Application.QuerieModel;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, List<PatientUpdatedEvent>>
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

        public async Task<List<PatientUpdatedEvent>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                //string patientIdCacheKey = $"PatientUpdatedEvent_{request.PatientId}";
                //var cachedData = await _cache.GetStringAsync(patientIdCacheKey);
                //if (!string.IsNullOrEmpty(cachedData))
                //{
                //    _logger.LogInformation("Returning patient list from Redis cache.");
                //    return JsonSerializer.Deserialize<List<PatientUpdatedEvent>>(cachedData);
                //}
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);

                if (patient == null)
                {
                    _logger.LogWarning($"Patient with ID {request.PatientId} not found.");
                    return new List<PatientUpdatedEvent>();
                }

                //var serializedPatients = JsonSerializer.Serialize(patient);
                //await _cache.SetStringAsync(
                //    patientIdCacheKey,
                //    serializedPatients,
                //    new DistributedCacheEntryOptions
                //    {
                //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                //    });

                //_logger.LogInformation("Patient list cached to Redis successfully.");
                
                var patientEvent = new PatientUpdatedEvent(
                    patient.PatientId,
                    patient.Name,
                    patient.Age,
                    patient.Gender,
                    patient.Department,
                    patient.RegisteredAt
                );

                return new List<PatientUpdatedEvent> { patientEvent };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while retrieving patient with ID {request.PatientId}.");
                return new List<PatientUpdatedEvent>();
            }
        }
    }
}
