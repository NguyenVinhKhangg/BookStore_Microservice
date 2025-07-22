using System.Text.Json.Serialization;

namespace BookClient.Models
{
    public class Category
    {
        [JsonPropertyName("categoryID")]
        public int CategoryID { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public string Description { get; set; }  // Nếu cần hiển thị thêm mô tả
        public bool IsActive { get; set; }  // Nếu cần lọc theo trạng thái


    }
}