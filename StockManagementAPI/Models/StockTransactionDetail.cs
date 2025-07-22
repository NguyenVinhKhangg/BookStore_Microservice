using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementApi.Models
{
    public class StockTransactionDetail
    {
        [Key]
        public int DetailID { get; set; }
        
        [Required]
        public int TransactionID { get; set; }
        
        [ForeignKey("TransactionID")]
        public StockTransaction Transaction { get; set; }
        
        [Required]
        public int BookID { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice { get; set; }
        
        [MaxLength(255)]
        public string? Note { get; set; }
    }
}