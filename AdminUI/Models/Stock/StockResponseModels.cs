using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Stock
{
    public class StockTransactionResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public StockTransactionViewModel Data { get; set; }
    }
    
    public class StockTransactionsListResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<StockTransactionViewModel> Data { get; set; } = new();
        public int TotalCount { get; set; }
    }
    
    public class UpdateTransactionStatusViewModel
    {
        [Required]
        public string Status { get; set; }
        public string Note { get; set; }
    }
}