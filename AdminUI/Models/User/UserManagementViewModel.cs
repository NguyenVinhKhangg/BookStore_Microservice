using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AdminUI.Models.User
{
    public class UserManagementViewModel
    {
        [JsonPropertyName("userID")]
        public int UserID { get; set; }

        [Display(Name = "Full Name")]
        [JsonPropertyName("fullname")] // ✅ Match với UserDTO
        public string FullName { get; set; }

        [Display(Name = "Email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        [JsonPropertyName("phonenumber")] // ✅ Match với UserDTO
        public string PhoneNumber { get; set; }

        [Display(Name = "Address")]
        [JsonPropertyName("address")]
        public string Address { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        [JsonPropertyName("birthDay")]
        public DateTime? BirthDay { get; set; }

        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }

        [Display(Name = "Role")]
        [JsonPropertyName("roleName")] // ✅ Match với UserDTO
        public string RoleName { get; set; }

        [Display(Name = "Status")]
        [JsonPropertyName("isDeactivated")] // ✅ Match với UserDTO
        public bool IsDeactivated { get; set; }

        [Display(Name = "Created At")]
        [JsonPropertyName("createdAt")] // ✅ Match với UserDTO
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Updated At")]
        [JsonPropertyName("updatedAt")] // ✅ Match với UserDTO
        public DateTime UpdatedAt { get; set; }
    }
}