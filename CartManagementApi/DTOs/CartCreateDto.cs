using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartCreateDto
    {
        [Required(ErrorMessage = "UserID là bắt buộc")]
        public int UserID { get; set; }

        // Optional: Cho phép tạo cart với items ngay lập tức
        public List<CartItemCreateDto>? CartItems { get; set; }
    }
}