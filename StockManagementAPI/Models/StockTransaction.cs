using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementApi.Models
{
    public class StockTransaction
    {
        [Key]
        public int TransactionID { get; set; }
        
        [Required]
        public int CreatedBy { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string TransactionType { get; set; } // StockIn, StockOut, Adjustment
        
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public int? ApprovedBy { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
        
        [MaxLength(255)]
        public string? Note { get; set; }
        
        public virtual ICollection<StockTransactionDetail> Details { get; set; } = new List<StockTransactionDetail>();
    }
}