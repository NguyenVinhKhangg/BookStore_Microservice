using System.ComponentModel.DataAnnotations;

namespace BookClient.Models.Profile
{
    public class ProfileViewModel
    {
        public int UserID { get; set; }
        
        [Display(Name = "Full Name")]
        public string Fullname { get; set; }
        
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        [Display(Name = "Address")]
        public string Address { get; set; }
        
        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
        
        [Display(Name = "Role")]
        public string RoleName { get; set; }
        
        public bool IsDeactivated { get; set; }
    }
}