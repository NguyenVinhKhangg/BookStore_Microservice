using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminUI.Models.Category
{
    public class CategoryViewModel
    {
        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }
    }
}
