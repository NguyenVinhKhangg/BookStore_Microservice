using System.ComponentModel.DataAnnotations;

namespace ReviewsApi.DTOs
{
    public class ReviewCreateDto
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ReviewUpdateDto
    {
        [Required]
        public int ReviewID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool IsActive { get; set; }
    }

    public class ReviewReadDto
    {
        public int ReviewID { get; set; }
        public int BookID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public bool IsActive { get; set; }
    }
}