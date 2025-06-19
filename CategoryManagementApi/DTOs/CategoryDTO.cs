using System.ComponentModel.DataAnnotations;

namespace CategoryManagementApi.DTOs
{
    public class CategoryDTO
    {
        [Key]
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}