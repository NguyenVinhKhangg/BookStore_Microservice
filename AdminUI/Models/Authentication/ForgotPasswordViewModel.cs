using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.Authentication
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
