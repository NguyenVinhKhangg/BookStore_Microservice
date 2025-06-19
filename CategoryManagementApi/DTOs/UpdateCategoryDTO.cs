using System.ComponentModel.DataAnnotations;

namespace CategoryManagementApi.DTOs
{
    public class UpdateCategoryDTO
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}