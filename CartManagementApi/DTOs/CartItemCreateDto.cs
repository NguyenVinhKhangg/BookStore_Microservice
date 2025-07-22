using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartItemCreateDto
    {
        [Required(ErrorMessage = "CartID là bắt buộc")]
        public int CartID { get; set; }

        [Required(ErrorMessage = "BookID là bắt buộc")]
        public int BookID { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0")]
        public decimal Price { get; set; }
    }
}