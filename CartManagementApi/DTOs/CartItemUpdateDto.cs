using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartItemUpdateDto
    {
        [Required]
        public int CartItemID { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        // ✅ Allow price update if needed
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal? Price { get; set; }
    }
}