using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.DTOs
{
    public class UserDTO
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string Fullname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Phonenumber { get; set; }

        [Required]
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public bool IsDeactivated { get; set; }

        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
    }
}
