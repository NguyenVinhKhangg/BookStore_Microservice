using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.User
{
    public class UpdateUserViewModel
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        // ✅ FIXED: Remove validation attributes - handle in controller
        [Display(Name = "Password (leave blank to keep unchanged)")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and contain 10 digits")]
        [Display(Name = "Phone Number")]
        public string Phonenumber { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }

      
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [Required(ErrorMessage = "Role is required")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}