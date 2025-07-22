using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Cart
{
    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public int BookID { get; set; }

        [Display(Name = "Book Title")]
        public string BookTitle { get; set; }

        [Display(Name = "Author")]
        public string AuthorName { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Discount")]
        public decimal Discount { get; set; }

        [Display(Name = "Final Price")]
        public decimal FinalPrice => UnitPrice * (1 - Discount / 100);

        [Display(Name = "Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Display(Name = "Total")]
        public decimal Total => FinalPrice * Quantity;

        [Display(Name = "Stock")]
        public int Stock { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
