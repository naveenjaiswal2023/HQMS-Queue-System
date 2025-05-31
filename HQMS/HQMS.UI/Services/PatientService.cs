using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using Microsoft.Extensions.Options;

namespace HospitalQueueSystem.Web.Services
{
    // Services/PatientService.cs
    public class PatientService : IPatientService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;

        public PatientService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IOptions<ApiSettings> apiSettings)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            if (apiSettings?.Value == null)
                throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured properly.");
            _apiBaseUrl = apiSettings.Value.BaseUrl;
            
        }

        private void AddJwtToken()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<List<PatientModel>> GetAllAsync()
        {
            AddJwtToken();
            var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Patient/GetAllPatients");
            return await _httpClient.GetFromJsonAsync<List<PatientModel>>(endpoint);
        }

        public async Task<PatientModel> GetByIdAsync(Guid id)
        {
            AddJwtToken();
            var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Patient");
            return await _httpClient.GetFromJsonAsync<PatientModel>($"{endpoint}/{id}");
        }

        public async Task CreateAsync(PatientModel patient)
        {
            AddJwtToken();
            var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Patient/RegisterPatient");
            await _httpClient.PostAsJsonAsync(endpoint, patient);
        }

        public async Task UpdateAsync(Guid id, PatientModel patient)
        {
            AddJwtToken();
            var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Patient");
            await _httpClient.PutAsJsonAsync($"{_apiBaseUrl}/{id}", endpoint);
        }

        public async Task DeleteAsync(Guid id)
        {
            AddJwtToken();
            var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Patient");
            await _httpClient.DeleteAsync($"{endpoint}/{id}");
        }
    }

}
