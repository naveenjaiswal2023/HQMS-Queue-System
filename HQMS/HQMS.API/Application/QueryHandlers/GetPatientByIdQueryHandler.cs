using HQMS.API.Application.QuerieModel;
using HQMS.API.Application.QueriesModel;
using HQMS.API.Domain.Interfaces;
using HQMS.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HQMS.API.Application.QueryHandlers
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, List<PatientUpdatedEvent>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetPatientByIdQueryHandler> _logger;

        public GetPatientByIdQueryHandler(
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<GetPatientByIdQueryHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<List<PatientUpdatedEvent>> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            string cacheKey = $"PatientUpdated:{request.PatientId}";

            try
            {
                // ✅ Try from Cache
                var cached = await _cacheService.GetAsync<List<PatientUpdatedEvent>>(cacheKey);
                if (cached != null)
                {
                    _logger.LogInformation($"Returned patient {request.PatientId} from cache.");
                    return cached;
                }

                // ❌ Not cached — fetch from DB
                var patient = await _unitOfWork.PatientRepository.GetByIdAsync(request.PatientId);
                if (patient == null)
                {
                    _logger.LogWarning($"Patient with ID {request.PatientId} not found.");
                    return new List<PatientUpdatedEvent>();
                }

                var patientEvent = new PatientUpdatedEvent(
                    patient.PatientId,
                    patient.Name,
                    patient.Age,
                    patient.Gender,
                    patient.DepartmentId,
                    patient.PhoneNumber,
                    patient.Email,
                    patient.Address,
                    patient.BloodGroup,
                    patient.HospitalId,
                    patient.PrimaryDoctorId
                );

                var result = new List<PatientUpdatedEvent> { patientEvent };

                // ✅ Cache for 10 minutes
                await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(10));

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving patient with ID {request.PatientId}.");
                return new List<PatientUpdatedEvent>();
            }
        }
    }
}
