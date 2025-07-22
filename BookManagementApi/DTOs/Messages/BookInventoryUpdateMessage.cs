namespace BookManagementApi.DTOs.Messages
{
    public class BookInventoryUpdateMessage
    {
        public int BookId { get; set; }
        public int QuantityChange { get; set; }
        public decimal UnitPrice { get; set; }
        public int TransactionId { get; set; }
        public string TransactionType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}