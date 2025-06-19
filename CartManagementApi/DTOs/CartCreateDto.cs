using System;
using System.ComponentModel.DataAnnotations;

namespace CartManagementApi.DTOs
{
    public class CartCreateDto
    {
        [Required]
        public int UserID { get; set; }
    }

    public class CartUpdateDto
    {
        [Required]
        public int CartID { get; set; }

        [Required]
        public int UserID { get; set; }
    }

    public class CartReadDto
    {
        [Key]
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
