namespace BookManagementApi.Models
{
    public class Orders
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int? PaymentId { get; set; }
        public int? ShippingId { get; set; }
        public string? OrderStatus { get; set; }
        public string? Note { get; set; }
    }
}