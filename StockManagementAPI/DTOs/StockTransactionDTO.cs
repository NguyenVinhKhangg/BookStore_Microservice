namespace StockManagementAPI.DTOs
{
    public class StockTransactionDTO
    {
        public int TransactionID { get; set; }
        public int CreatedBy { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? Note { get; set; }
        public ICollection<StockTransactionDetailDTO> Details { get; set; }
    }
}
