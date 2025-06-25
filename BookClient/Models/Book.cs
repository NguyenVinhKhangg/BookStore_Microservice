using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BookClient.Models
{
    public class Book
    {
        [JsonPropertyName("bookID")]
        public int BookID { get; set; }

        [JsonPropertyName("title")]
        [Required(ErrorMessage = "Tên sách không được để trống.")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("isbn")]
        [Required(ErrorMessage = "ISBN không được để trống.")]
        public string ISBN { get; set; }

        [JsonPropertyName("price")]
        [Required(ErrorMessage = "Giá không được để trống.")]
        public decimal Price { get; set; }

        [JsonPropertyName("discount")]
        public decimal Discount { get; set; }

        [JsonPropertyName("stock")]
        [Required(ErrorMessage = "Số lượng kho không được để trống.")]
        public int Stock { get; set; }

        [JsonPropertyName("categoryID")]
        [Required(ErrorMessage = "Danh mục không được để trống.")]
        public int CategoryID { get; set; }

        [JsonPropertyName("categoryName")]
        public string CategoryName { get; set; }

        public int AuthorID { get; set; }

        [JsonPropertyName("authorName")]
        [Required(ErrorMessage = "Tên tác giả không được để trống.")]
        public string AuthorName { get; set; }

        public int PublisherID { get; set; }

        [JsonPropertyName("publisherName")]
        [Required(ErrorMessage = "Tên nhà xuất bản không được để trống.")]
        public string PublisherName { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
    }
}