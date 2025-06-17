namespace BookManagementApi.DTOs
{
    public class OrdersDTO
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int? PaymentId { get; set; }
        public int? ShippingId { get; set; }
        public string? OrderStatus { get; set; } // Nullable
        public string? Note { get; set; } // Nullable
    }

    public class UpdateStatusOrderDTO
    {
        public int OrderId { get; set; }
        public string? Status { get; set; } // Nullable
    }
}