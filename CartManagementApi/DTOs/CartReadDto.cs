using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartReadDto
    {
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public List<CartItemReadDto> CartItems { get; set; } = new List<CartItemReadDto>();

        // Computed properties
        public int TotalItems => CartItems?.Sum(item => item.Quantity) ?? 0;
        public decimal TotalAmount => CartItems?.Sum(item => item.TotalPrice) ?? 0;
    }
}