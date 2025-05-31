using HospitalQueueSystem.Web.Interfaces;
using HospitalQueueSystem.Web.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace HospitalQueueSystem.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl;

        public AuthService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> apiSettings)
        {
            if (apiSettings?.Value == null)
                throw new ArgumentNullException(nameof(apiSettings), "ApiSettings is not configured properly.");

            _httpClientFactory = httpClientFactory;
            _apiBaseUrl = apiSettings.Value.BaseUrl;
        }

        public async Task<(bool IsSuccess, TokenDto? TokenData, string? ErrorMessage)> LoginAsync(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return (false, null, "Invalid login input.");
            }

            var client = _httpClientFactory.CreateClient();

            var loginData = new
            {
                Email = model.Email,
                Password = model.Password
            };

            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var endpoint = new Uri(new Uri(_apiBaseUrl.TrimEnd('/') + "/"), "Auth/login");
                var response = await client.PostAsync(endpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var tokenResponse = JsonSerializer.Deserialize<ApiResponse<TokenDto>>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (tokenResponse == null || tokenResponse.Data == null)
                    {
                        return (false, null, "Invalid response from server.");
                    }

                    return (true, tokenResponse.Data, null);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, null, "Invalid email or password.");
                }
                else
                {
                    return (false, null, $"Login failed: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                // You could log httpEx.Message if needed
                return (false, null, "Unable to connect to the authentication server. Please try again later.");
            }
            catch (Exception ex)
            {
                // Log ex if needed
                return (false, null, "An unexpected error occurred. Please try again.");
            }
        }
    }
}
