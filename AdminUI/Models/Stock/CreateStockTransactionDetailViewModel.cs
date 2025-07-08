using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Stock
{
    public class CreateStockTransactionDetailViewModel
    {
        [Required(ErrorMessage = "Book ID is required")]
        [Display(Name = "Book")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Note")]
        [StringLength(255)]
        public string Note { get; set; }
    }
}
