using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Cart
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        // Computed properties
        public int TotalItems => CartItems?.Sum(item => item.Quantity) ?? 0;
        public decimal TotalAmount => CartItems?.Sum(item => item.TotalPrice) ?? 0;
        public decimal TotalDiscount => CartItems?.Sum(item =>
            item.BookDiscount.HasValue ? (item.TotalPrice - item.TotalDiscountedPrice) : 0) ?? 0;
        public decimal FinalAmount => TotalAmount - TotalDiscount;
    }

    public class CartItemViewModel
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime AddedAt { get; set; }

        // Book details
        public string BookTitle { get; set; } = string.Empty;
        public string BookISBN { get; set; } = string.Empty;
        public string BookImageUrl { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public decimal? BookDiscount { get; set; }

        // Computed properties
        public decimal TotalPrice => Quantity * Price;
        public decimal DiscountedPrice => BookDiscount.HasValue ? Price * (1 - BookDiscount.Value / 100) : Price;
        public decimal TotalDiscountedPrice => Quantity * DiscountedPrice;
    }

    public class AddToCartRequest
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; }
    }

    public class UpdateCartItemRequest
    {
        [Required]
        public int CartItemID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // ✅ Thêm Price để cập nhật nếu cần
        public decimal? Price { get; set; }
    }
}