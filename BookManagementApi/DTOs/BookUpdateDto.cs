namespace BookManagementApi.DTOs
{
    public class BookUpdateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0;
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
