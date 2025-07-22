using System.ComponentModel.DataAnnotations;

namespace CouponsApi.DTOs
{
    public class CouponDTO
    {
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        [Range(0.01, 99.99)]
        public decimal DiscountPercent { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CouponUpdateDto
    {
        [Required]
        public int CouponID { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        [Range(0.01, 99.99)]
        public decimal DiscountPercent { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }

    public class CouponReadDto
    {
        [Required]
        [Key]
        public int CouponID { get; set; }
        [Required]
        [StringLength(50)]
        public string Code { get; set; }
        [Required]
        [Range(0.01, 99.99)]
        public decimal DiscountPercent { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}