using System.ComponentModel.DataAnnotations;

namespace UserManagementApi.Models
{
    public class Roles
    {
        [Key]
        public int RoleID { get; set; }

        [Required]
        [StringLength(20)]
        public string RoleName { get; set; }
    }
}
