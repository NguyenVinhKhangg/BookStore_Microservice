using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookImgManagementApi.Models
{
    public class BooksImg
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }

        [Required]
        public int BookID { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;

        [MaxLength(255)]
        public string? Caption { get; set; }

        public bool IsCover { get; set; } = false;

        public int SortOrder { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    }
}
