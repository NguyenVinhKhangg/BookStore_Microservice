using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Cart
{
    public class UpdateCartItemViewModel
    {
        [Required]
        public int CartID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
