using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartUpdateDto
    {
        [Required]
        public int CartID { get; set; }

        [Required(ErrorMessage = "UserID là bắt buộc")]
        public int UserID { get; set; }

        // Có thể cập nhật thời gian modified
        public DateTime? ModifiedAt { get; set; }
    }
}