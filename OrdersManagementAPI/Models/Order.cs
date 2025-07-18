using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrdersManagementApi.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = null!;
    }
}