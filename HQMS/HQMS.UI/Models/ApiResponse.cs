using System.Text.Json.Serialization;

namespace HospitalQueueSystem.Web.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("isSuccess")]
        public bool Succeeded { get; set; }

        [JsonPropertyName("data")]
        public T? Data { get; set; }

        [JsonPropertyName("errorMessage")]
        public string? Message { get; set; }
    }

}
