namespace BookManagementApi.DTOs
{
    public class BookCreateDto
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string ISBN { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0;
        // Bỏ Stock - không cho phép client set stock khi tạo
        public int CategoryID { get; set; }
        public string AuthorName { get; set; }
        public string PublisherName { get; set; }
        public string? ImageUrl { get; set; }
        // Bỏ IsActive - sẽ được set mặc định trong service
    }
}
