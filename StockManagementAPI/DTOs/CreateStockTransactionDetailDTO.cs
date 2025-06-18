using System.ComponentModel.DataAnnotations;

namespace StockManagementAPI.DTOs
{
    public class CreateStockTransactionDetailDTO
    {
        [Required(ErrorMessage = "Book ID is required")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0, 9999999.99, ErrorMessage = "Unit price must be between 0 and 9999999.99")]
        public decimal UnitPrice { get; set; }

        [StringLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }
}
