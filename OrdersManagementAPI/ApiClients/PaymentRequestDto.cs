using System.ComponentModel.DataAnnotations;

namespace OrdersManagementApi.DTOs
{
    public class PaymentRequestDto
    {
        [Required]
        public int OrderID { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string? PaymentMethod { get; set; } // Đảm bảo nullable
    }
}