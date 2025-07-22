using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookClient.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public int CartID { get; set; } // Liên kết với giỏ hàng

        [Required]
        public string UserID { get; set; } = string.Empty; // Đổi thành string để khớp với view

        public DateTime OrderDate { get; set; } = DateTime.UtcNow; // Ngày đặt hàng

        public string Status { get; set; } = "Pending"; // Trạng thái đơn hàng, tránh null

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>(); // Danh sách mục đơn hàng
    }

    public class OrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; } // Liên kết với Order

        [Required]
        public string BookName { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }
    }
}