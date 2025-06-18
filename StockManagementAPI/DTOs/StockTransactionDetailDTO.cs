namespace StockManagementAPI.DTOs
{
    public class StockTransactionDetailDTO
    {
        public int DetailID { get; set; }
        public int TransactionID { get; set; }
        public int BookID { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public string? Note { get; set; }
    }
 
}
