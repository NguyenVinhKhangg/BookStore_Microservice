using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Profile
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        
        [Required(ErrorMessage = "Full name is required")]
        [Display(Name = "Full Name")]
        public string Fullname { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Address is required")]
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"^0\d{9}$", ErrorMessage = "Phone number must start with 0 and contain 10 digits")]
        [Display(Name = "Phone Number")]
        public string Phonenumber { get; set; }
        
        [Display(Name = "Birth Day")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
        
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        
        public bool IsDeactivated { get; set; }
    }
}