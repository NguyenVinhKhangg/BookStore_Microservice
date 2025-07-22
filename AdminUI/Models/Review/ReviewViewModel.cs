using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Review
{
    public class ReviewViewModel
    {
        public int ReviewID { get; set; }

        [Required(ErrorMessage = "Sách là bắt buộc")]
        [Display(Name = "Sách")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Người dùng là bắt buộc")]
        [Display(Name = "Người dùng")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Điểm là bắt buộc")]
        [Range(1, 5, ErrorMessage = "Điểm phải từ 1 đến 5")]
        [Display(Name = "Điểm")]
        public int Rating { get; set; }

        [StringLength(500, ErrorMessage = "Bình luận không được vượt quá 500 ký tự")]
        [Display(Name = "Bình luận")]
        public string? Comment { get; set; }

        [Display(Name = "Ngày đánh giá")]
        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; }

        [Display(Name = "Hoạt động")]
        public bool IsActive { get; set; }

        public List<SelectListItem> BookOptions { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();
    }

    public class ReviewSearchFilterViewModel
    {
        [Display(Name = "Tìm kiếm")]
        public string? SearchTerm { get; set; }

        [Display(Name = "Trạng thái")]
        public bool? StatusFilter { get; set; }

        [Display(Name = "Sách")]
        public int? BookFilter { get; set; }
    }
}
