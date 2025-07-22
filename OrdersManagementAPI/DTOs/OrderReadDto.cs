using System.ComponentModel.DataAnnotations;

namespace OrdersManagementApi.DTOs
{
    // ✅ Read DTO đơn giản
    public class OrderReadDto
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }

        // Admin info
        public DateTime? ConfirmedAt { get; set; }
        public int? ConfirmedBy { get; set; }
        public string? AdminNotes { get; set; }

        // Items
        public List<OrderItemReadDto> OrderItems { get; set; } = new List<OrderItemReadDto>();

        // Computed properties
        public int TotalItems => OrderItems.Sum(item => item.Quantity);
    }

    // ✅ Checkout DTO đơn giản từ cart
    public class CheckoutDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public List<CheckoutItemDto> Items { get; set; } = new List<CheckoutItemDto>();

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class CheckoutItemDto
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // Optional enrichment
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
    }

    // ✅ Order Item DTO
    public class OrderItemReadDto
    {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
        public string? BookImageUrl { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // ✅ Create Order DTO
    public class OrderCreateDto
    {
        [Required]
        public int UserID { get; set; }

        [Required]
        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();

        public string? Notes { get; set; }
    }

    public class OrderItemCreateDto
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }
    }

    // ✅ Update Order Status DTO
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = string.Empty; // Confirmed, Processing, Shipped, Delivered, Cancelled

        public string? AdminNotes { get; set; }
    }

    // ✅ Order Summary DTO cho admin (đơn giản)
    public class OrderSummaryDto
    {
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItems { get; set; }
    }
}