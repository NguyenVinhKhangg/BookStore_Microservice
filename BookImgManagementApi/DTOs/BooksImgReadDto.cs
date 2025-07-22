using System.ComponentModel.DataAnnotations;

namespace BookImgManagementApi.DTOs
{
    public class BooksImgReadDto
    {
        [Key]
        public int ImageID { get; set; }
        public int BookID { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public int SortOrder { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class BooksImgCreateDto
    {
        [Required]
        public int BookID { get; set; }
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;
        [MaxLength(255)]
        public string? Caption { get; set; }
        public bool IsCover { get; set; } = false;
        public int SortOrder { get; set; } = 0;
    }

    public class BooksImgUpdateDto
    {
        [Required]
        public int ImageID { get; set; }
        [Required]
        public int BookID { get; set; }
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;
        [MaxLength(255)]
        public string? Caption { get; set; }
        public bool IsCover { get; set; }
        public int SortOrder { get; set; }

    }
}
