using System.ComponentModel.DataAnnotations;

namespace CartItemManagementApi.DTOs
{
    public class CartItemCreateDto
    {
  
        public int CartID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemUpdateDto
    {   
        public int CartItemID { get; set; }
        public int Quantity { get; set; }
    }

    public class CartItemReadDto
    {
        [Key]
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int BookID { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; }
        public bool IsActive { get; set; }
    }
}