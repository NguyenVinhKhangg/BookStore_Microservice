using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Validations;

namespace BussinessObject.DTO.UserDTO
{
    public class CreateUserDTO
    {
        [Required(ErrorMessage = "Username cannot be empty")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Address cannot be empty")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty")]
        [Range(0100000000, 0999999999, ErrorMessage = "Phone number must start with 0 and contain 10 digits")]
        [Display(Name = "Phone Number")]
        public int Phonenumber { get; set; }

        [Required(ErrorMessage = "Password cannot be empty")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 30 characters")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-Z]).*$", ErrorMessage = "Password must contain at least 1 digit")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [PastDate(ErrorMessage = "Birth date must be in the past")]
        [Display(Name = "Birth Day")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [Required(ErrorMessage = "Role cannot be empty")]
        [Range(2, 3, ErrorMessage = "Role must be either Staff or User")]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}
