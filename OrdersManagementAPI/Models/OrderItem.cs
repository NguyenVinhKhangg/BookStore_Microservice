using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersManagementApi.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [ForeignKey(nameof(OrderID))]
        public virtual Order Order { get; set; } = null!;

        [Required]
        public int BookID { get; set; } // ✅ SỬA: Đổi từ ProductID thành BookID

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        // ✅ Book information (enriched từ BookAPI)
        public string? BookTitle { get; set; }
        public string? BookISBN { get; set; }
        public string? BookImageUrl { get; set; }

        // ✅ Computed property
        public decimal TotalPrice => Quantity * UnitPrice;
    }
}