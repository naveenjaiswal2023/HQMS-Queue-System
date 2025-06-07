using System.Text.Json.Serialization;

namespace HospitalQueueSystem.Web.Models
{
    public class PatientModel
    {
        [JsonPropertyName("patientId")]
        public Guid PatientId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("age")]
        public int Age { get; set; }

        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;

        [JsonPropertyName("department")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("registeredAt")]
        public DateTime RegisteredAt { get; set; }
    }

}
