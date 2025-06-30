using HQMS.API.Domain.Interfaces;
using HQMS.Application.Queries;
using HQMS.Domain.Events;
using HQMS.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HQMS.Application.QueryHandlers
{
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, List<PatientRegisteredEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetAllPatientsQueryHandler> _logger;
        private const string PatientListCacheKey = "Patient_CacheKey";
        private readonly IPatientRepository _patientRepository;

        public GetAllPatientsQueryHandler(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<GetAllPatientsQueryHandler> logger, IPatientRepository patientRepository)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
            _patientRepository = patientRepository;
        }

        public async Task<List<PatientRegisteredEvent>> Handle(GetAllPatientsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Try from Cache
                var cachedPatients = await _cacheService.GetAsync<List<PatientRegisteredEvent>>(PatientListCacheKey);
                if (cachedPatients != null)
                {
                    _logger.LogInformation("Returning patient list from Redis cache.");
                    return cachedPatients;
                }

                // 2. Fetch from repository
                var patients = await _patientRepository.GetAllPatientsAsync(cancellationToken);

                if (!patients.Any())
                {
                    _logger.LogWarning("No patients found in the database.");
                    return new List<PatientRegisteredEvent>();
                }

                // 3. Cache result
                await _cacheService.SetAsync(PatientListCacheKey, patients, TimeSpan.FromMinutes(5));
                _logger.LogInformation("Patient list cached to Redis successfully.");

                return patients;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving patient list.");
                return new List<PatientRegisteredEvent>();
            }
        }
    }
}
