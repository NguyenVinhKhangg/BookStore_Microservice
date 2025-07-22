using System.ComponentModel.DataAnnotations;

namespace StockManagementAPI.DTOs
{
    public class UpdateTransactionStatusDTO
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Approved|Rejected)$",
            ErrorMessage = "Status must be one of: Approved, Rejected")]
        public string Status { get; set; }

        [StringLength(255, ErrorMessage = "Note cannot exceed 255 characters")]
        public string? Note { get; set; }
    }
}
