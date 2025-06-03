using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using Microsoft.AspNetCore.Identity.Data;

namespace HospitalQueueSystem.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApiService _apiService;

        public AuthService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<(bool IsSuccess, TokenDto? TokenData, string? ErrorMessage)> LoginAsync(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return (false, null, "Email and password are required.");

            try
            {
                var response = await _apiService.PostAsync<TokenDto>("auth/login", model);

                if (response?.Data == null)
                    return (false, null, "Invalid login response from API.");

                TokenDto? token = response.Data;
                return (true, token, null);
            }
            catch (HttpRequestException)
            {
                return (false, null, "Authentication server unreachable.");
            }
            catch (Exception)
            {
                return (false, null, "Unexpected error occurred during login.");
            }
        }

    }
}
