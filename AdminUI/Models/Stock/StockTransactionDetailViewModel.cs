using System.ComponentModel.DataAnnotations;

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
        public string BookTitle { get; set; }
        
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