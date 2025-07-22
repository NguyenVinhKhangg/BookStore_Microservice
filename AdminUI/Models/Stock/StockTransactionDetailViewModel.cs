using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminUI.Models.Stock
{
    public class StockTransactionDetailViewModel
    {
        public int DetailID { get; set; }
        
        [Display(Name = "Transaction ID")]
        public int TransactionID { get; set; }
        
        [Display(Name = "Book ID")]
        public int BookID { get; set; }

        [Display(Name = "Book Title")]
        [JsonPropertyName("bookName")] // ✅ THÊM: Map từ bookName trong API
        public string? BookTitle { get; set; }

        [Display(Name = "Book ISBN")]
        [JsonPropertyName("bookISBN")] // ✅ THÊM: Map từ bookISBN trong API
        public string? BookISBN { get; set; }

        [Display(Name = "Book Code")]
        public string BookCode { get; set; }
        
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }
        
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        
        [Display(Name = "Total")]
        public decimal Total => Quantity * UnitPrice;
        
        [Display(Name = "Note")]
        public string Note { get; set; }
    }
}