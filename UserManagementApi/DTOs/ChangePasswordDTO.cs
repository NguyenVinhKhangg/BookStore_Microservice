using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessObject.DTO.UserDTO
{
     public class ChangePasswordDTO
    {
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 30 characters")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[a-zA-Z]).*$", ErrorMessage = "Password must contain at least 1 digit")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm new password is required")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}
