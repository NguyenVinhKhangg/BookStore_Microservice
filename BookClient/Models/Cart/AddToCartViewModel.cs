using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Cart
{
    public class AddToCartViewModel
    {
        [Required]
        public int BookID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }
}
