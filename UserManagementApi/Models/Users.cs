using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace UserManagementApi.Models
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [StringLength(15)]
        public string PhoneNumber { get; set; }

        public DateTime? BirthDay { get; set; }

        [StringLength(255)]
        public string Address { get; set; }

        [ForeignKey("Role")]
        public int RoleID { get; set; }

        public bool DeactivatedStatus { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Roles Role { get; set; }
    }
}
