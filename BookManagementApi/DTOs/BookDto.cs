using System;

namespace BookManagementApi.DTOs
{
    public class BookDto
    {
        public int BookID { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Stock { get; set; }
        public int CategoryID { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public int PublisherID { get; set; }
        public string PublisherName { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
