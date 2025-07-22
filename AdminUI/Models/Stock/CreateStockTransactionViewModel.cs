using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Stock
{
    public class CreateStockTransactionViewModel
    {
        [Required(ErrorMessage = "Transaction type is required")]
        [Display(Name = "Transaction Type")]
        public string TransactionType { get; set; } = "StockIn";
        
        [Display(Name = "Note")]
        [StringLength(255)]
        public string Note { get; set; }
        
        [Required]
        [MinLength(1, ErrorMessage = "At least one item is required")]
        public List<CreateStockTransactionDetailViewModel> Details { get; set; } = new();
    }
    
    
}