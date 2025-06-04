using System.ComponentModel.DataAnnotations;

namespace CouponsApi.DTOs
{
    public class CouponCreateDto
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
        public int CouponID { get; set; }
        public string Code { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}