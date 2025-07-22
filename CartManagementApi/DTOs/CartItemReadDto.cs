using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartItemReadDto
    {
        public int CartItemID { get; set; }

        public int CartID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }

        public DateTime AddedAt { get; set; }

        // Enriched properties từ Book API
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
        public string? BookImageUrl { get; set; }
        public string? AuthorName { get; set; }
        public decimal? BookDiscount { get; set; }

        // Computed properties
        public decimal TotalPrice => Quantity * Price;
        public decimal DiscountedPrice => BookDiscount.HasValue ? Price * (1 - BookDiscount.Value / 100) : Price;
        public decimal TotalDiscountedPrice => Quantity * DiscountedPrice;
    }
}