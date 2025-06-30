//using HospitalQueueSystem.Domain.Events;

using HQMS.Web.Interfaces;
using HQMS.Web.Models;
using HQMS.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace HQMS.Web.Services
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

        public async Task<List<PatientModel>> GetAllAsync()
        {
            var response = await _apiService.GetAsync<List<PatientModel>>("Patient/GetAllPatients");
            return response?.Data ?? new List<PatientModel>();
        }

        public async Task<PatientModel?> GetByIdAsync(Guid id)
        {
            var response = await _apiService.GetAsync < List < PatientModel >>($"Patient/{id}");
            //return response?.Data ?? new List<PatientModel>();
            return response?.Data?.FirstOrDefault() ?? null;

        }
        public async Task CreateAsync(PatientModel patient)
        {
            var response = await _apiService.PostAsync<object>("Patient/RegisterPatient", patient);
            if (!response.IsSuccess)
                throw new Exception(response.ErrorMessage ?? "Failed to create patient.");
        }

        public async Task UpdateAsync(Guid id, PatientModel patient)
        {
            var response = await _apiService.PutAsync<object>($"Patient/{id}", patient);
            if (!response.IsSuccess)
                throw new Exception(response.ErrorMessage ?? "Failed to update patient.");
        }

        public async Task DeleteAsync(Guid id)
        {
            var response = await _apiService.DeleteAsync<object>($"Patient/{id}");
            if (!response.IsSuccess)
                throw new Exception(response.ErrorMessage ?? "Failed to delete patient.");
        }

    }
}
