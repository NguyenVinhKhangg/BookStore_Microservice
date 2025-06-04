using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserManagementApi.Validations;

namespace BussinessObject.DTO.UserDTO
{
    public class UpdateProfileDTO
    {

        [Required(ErrorMessage = "Username cannot be empty")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Address cannot be empty")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [PastDate(ErrorMessage = "Birth date must be in the past")]
        [Display(Name = "Birth Day")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }

        [Required(ErrorMessage = "Phone number cannot be empty")]
        [Range(0100000000, 0999999999, ErrorMessage = "Phone number must start with 0 and contain 10 digits")]
        [Display(Name = "Phone Number")]
        public int Phonenumber { get; set; }
    }
}
