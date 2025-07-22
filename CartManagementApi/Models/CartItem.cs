using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemID { get; set; }

        [Required]
        public int CartID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be non-negative")]
        public decimal Price { get; set; }

        public DateTime AddedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Cart Cart { get; set; }
    }
}