using System.Text.Json.Serialization;

namespace AdminUI.Models.Category
{
    public class CategoryResponseModel
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public CategoryViewModel? Data { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
