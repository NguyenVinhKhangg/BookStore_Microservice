using System.ComponentModel.DataAnnotations;

namespace StockManagementAPI.DTOs
{

    public class CreateStockTransactionDTO
    {
        [Required(ErrorMessage = "Transaction type is required")]
        [RegularExpression("^(StockIn|StockOut|Adjustment)$",
            ErrorMessage = "Transaction type must be one of: StockIn, StockOut, Adjustment")]
        public string TransactionType { get; set; }

        [StringLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }

        [Required(ErrorMessage = "At least one transaction detail is required")]
        [MinLength(1, ErrorMessage = "At least one transaction detail is required")]
        public ICollection<CreateStockTransactionDetailDTO> Details { get; set; }
    }
}
