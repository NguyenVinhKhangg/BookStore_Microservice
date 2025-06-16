namespace BookManagementApi.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int PublisherID { get; set; } // Bỏ
        public string PublisherName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true; // Bỏ
    }
}
