using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Book
{
    public class CreateBookViewModel
    {
        [Required(ErrorMessage = "Tên sách là bắt buộc")]
        [MaxLength(200, ErrorMessage = "Tên sách không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "ISBN là bắt buộc")]
        [MaxLength(20, ErrorMessage = "ISBN không được vượt quá 20 ký tự")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá phải từ 0-100%")]
        public decimal Discount { get; set; } = 0;

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên tác giả không được vượt quá 100 ký tự")]
        public string AuthorName { get; set; }

        [Required(ErrorMessage = "Tên nhà xuất bản là bắt buộc")]
        [MaxLength(100, ErrorMessage = "Tên nhà xuất bản không được vượt quá 100 ký tự")]
        public string PublisherName { get; set; }

        [MaxLength(255, ErrorMessage = "URL ảnh không được vượt quá 255 ký tự")]
        public string? ImageUrl { get; set; }
    }
}
