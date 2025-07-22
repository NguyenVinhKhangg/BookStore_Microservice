using System.ComponentModel.DataAnnotations;

namespace OrdersManagementApi.DTOs
{
    public class OrderReadDto
    {
        [Key]
        public int OrderID { get; set; }
        public int UserID { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Status { get; set; } // Thay đổi thành string?
    }

    public class OrderCreateDto
    {
        [Required]
        public int UserID { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; } // Thay đổi thành string?
    }

    public class OrderUpdateDto
    {
        [Required]
        public int OrderID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [MaxLength(50)]
        public string? Status { get; set; } // Thay đổi thành string?
    }
}