using HospitalQueueSystem.Domain.Events;
using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Linq;

namespace HospitalQueueSystem.Web.Services
{
    public class PatientService : IPatientService
    {
        private readonly ApiService _apiService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PatientService(ApiService apiService, IHttpContextAccessor httpContextAccessor)
        {
            _apiService = apiService;
            _httpContextAccessor = httpContextAccessor;
        }

        //private void SetJwtHeader()
        //{
        //    var token = _httpContextAccessor.HttpContext?.Session.GetString("JwtToken");
        //    if (!string.IsNullOrEmpty(token))
        //    {
        //        _apiService.SetAuthorizationHeader(token);
        //    }
        //}

        public async Task<List<PatientModel>> GetAllAsync()
        {
            var response = await _apiService.GetAsync<List<PatientModel>>("Patient/GetAllPatients");
            return response?.Data ?? new List<PatientModel>();
        }

        public async Task<PatientModel?> GetByIdAsync(Guid id)
        {
            var response = await _apiService.GetAsync<ApiResponse<List<PatientRegisteredEvent>>>($"Patient/{id}");

            if (response?.Succeeded != true || response.Data == null)
            {
                return null;
            }

            //var eventModel = response.Data.FirstOrDefault();
            //if (eventModel == null) return null;

            return new PatientModel
            {
                //PatientId = eventModel.PatientId,
                //Name = eventModel.Name,
                //Age = eventModel.Age,
                //Gender = eventModel.Gender,
                //Department = eventModel.Department,
                //RegisteredAt = eventModel.RegisteredAt
            };
        }
        public async Task CreateAsync(PatientModel patient)
        {
            var response = await _apiService.PostAsync<object>("Patient/RegisterPatient", patient);
            if (!response.Succeeded)
                throw new Exception(response.Message ?? "Failed to create patient.");
        }

        public async Task UpdateAsync(Guid id, PatientModel patient)
        {
            var response = await _apiService.PutAsync<object>($"Patient/{id}", patient);
            if (!response.Succeeded)
                throw new Exception(response.Message ?? "Failed to update patient.");
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _apiService.DeleteAsync<object>($"Patient/{id}");
            if (!response.Succeeded)
                throw new Exception(response.Message ?? "Failed to delete patient.");
        }

    }
}
