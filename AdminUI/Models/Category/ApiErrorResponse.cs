using System.Text.Json.Serialization;

namespace AdminUI.Models.Category
{
    public class ApiErrorResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
