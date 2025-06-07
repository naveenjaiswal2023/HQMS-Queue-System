using HospitalQueueSystem.Web.Models;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace HospitalQueueSystem.Web.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;

        public ApiService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options, IHttpContextAccessor httpContextAccessor)
        {
            if (options?.Value == null || string.IsNullOrWhiteSpace(options.Value.BaseUrl))
                throw new ArgumentNullException(nameof(options), "API base URL is not configured.");

            _httpClientFactory = httpClientFactory;
            _baseUrl = options.Value.BaseUrl.TrimEnd('/');
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("AuthorizedClient");
        }

        private static StringContent SerializeContent(object data)
        {
            var json = JsonSerializer.Serialize(data);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static async Task<ApiResponse<T>?> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Request failed with status code: {response.StatusCode}.\nContent: {content}");
            }

            try
            {
                var result = JsonSerializer.Deserialize<ApiResponse<T>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return result;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to deserialize response JSON. Content: {content}", ex);
            }
        }


        private static Uri CombineUri(string baseUrl, string endpoint)
        {
            return new Uri(new Uri(baseUrl + "/"), endpoint.TrimStart('/'));
        }

        public async Task<ApiResponse<T>?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var fullUri = CombineUri(_baseUrl, endpoint);
            var response = await client.GetAsync(fullUri);
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T>?> PostAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var content = SerializeContent(data);
            var fullUri = CombineUri(_baseUrl, endpoint);
            var response = await client.PostAsync(fullUri, content);
            return await HandleResponse<T>(response);
        }

        public async Task<ApiResponse<T?>> PutAsync<T>(string endpoint, object data)
        {
            var client = CreateClient();
            var response = await client.PutAsync(CombineUri(_baseUrl, endpoint), SerializeContent(data));
            return await HandleResponse<T>(response);
        }
        public async Task<ApiResponse<T?>> DeleteAsync<T>(string endpoint)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(CombineUri(_baseUrl, endpoint));

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                // If no content returned, assume success with default T value
                return new ApiResponse<T?>
                {
                    IsSuccess = true,
                    Data = default,
                    ErrorMessage = "Deleted successfully."
                };
            }

            return await HandleResponse<T>(response);
        }

    }
}
