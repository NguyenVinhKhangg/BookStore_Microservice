using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Coupon
{
    public class CouponViewModel
    {
        public int CouponID { get; set; }

        [Required(ErrorMessage = "Mã giảm giá là bắt buộc")]
        [StringLength(50, ErrorMessage = "Mã giảm giá không được vượt quá 50 ký tự")]
        [Display(Name = "Mã giảm giá")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 100")]
        [Display(Name = "Phần trăm giảm giá")]
        public decimal DiscountPercent { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime ValidFrom { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn là bắt buộc")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày hết hạn")]
        public DateTime ValidTo { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }
    }

    public class CouponSearchFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string SearchTerm { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? StatusFilter { get; set; }

        [Display(Name = "Phần trăm giảm giá")]
        public decimal? DiscountFilter { get; set; }
    }
}
