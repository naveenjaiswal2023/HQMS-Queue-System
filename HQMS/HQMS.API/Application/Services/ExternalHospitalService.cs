using HQMS.API.Application.DTO;
using HQMS.API.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace HQMS.API.Application.Services
{
    public class ExternalHospitalService : IExternalHospitalService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ExternalHospitalService> _logger;
        private readonly IMemoryCache _cache;

        public ExternalHospitalService(HttpClient httpClient, ILogger<ExternalHospitalService> logger, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            const string cacheKey = "departments";

            if (_cache.TryGetValue(cacheKey, out List<DepartmentDto> cachedDepartments))
                return cachedDepartments;

            try
            {
                var response = await _httpClient.GetAsync("api/departments");
                response.EnsureSuccessStatusCode();

                var departments = await response.Content.ReadFromJsonAsync<List<DepartmentDto>>() ?? new();
                _cache.Set(cacheKey, departments, TimeSpan.FromHours(2)); // Cache for 2 hours

                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch departments.");
                return new();
            }
        }


        public async Task<List<DoctorSlotDto>> GetDoctorSlotsAsync(Guid doctorId)
        {
            string cacheKey = $"doctor_slots_{doctorId}";

            if (_cache.TryGetValue(cacheKey, out List<DoctorSlotDto> cachedSlots))
                return cachedSlots;

            try
            {
                var response = await _httpClient.GetAsync($"api/doctors/{doctorId}/slots");
                response.EnsureSuccessStatusCode();

                var slots = await response.Content.ReadFromJsonAsync<List<DoctorSlotDto>>() ?? new();
                _cache.Set(cacheKey, slots, TimeSpan.FromMinutes(15)); // Cache for 15 minutes

                return slots;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch doctor slots for doctorId: {DoctorId}", doctorId);
                return new();
            }
        }


        public async Task<List<AppointmentDto>> GetAppointmentsByPatientIdAsync(Guid patientId)
        {
            string cacheKey = $"appointments_patient_{patientId}";

            if (_cache.TryGetValue(cacheKey, out List<AppointmentDto> cachedAppointments))
                return cachedAppointments;

            try
            {
                var response = await _httpClient.GetAsync($"api/patients/{patientId}/appointments");
                response.EnsureSuccessStatusCode();

                var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentDto>>() ?? new();
                _cache.Set(cacheKey, appointments, TimeSpan.FromMinutes(5)); // Cache for 5 minutes

                return appointments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch appointments for patientId: {PatientId}", patientId);
                return new();
            }
        }
    }
}
