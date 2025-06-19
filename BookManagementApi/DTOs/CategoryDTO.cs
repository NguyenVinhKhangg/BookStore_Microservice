namespace BookManagementApi.DTOs
{
    public class CategoryDTO
    {
        public int CategoryID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
