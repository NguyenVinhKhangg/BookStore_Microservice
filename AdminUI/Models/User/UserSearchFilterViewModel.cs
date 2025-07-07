using System.ComponentModel.DataAnnotations;

namespace AdminUI.Models.User
{
    public class UserSearchFilterViewModel
    {
        [Display(Name = "Search")]
        public string SearchTerm { get; set; }

        [Display(Name = "Role")]
        public int? RoleFilter { get; set; }

        [Display(Name = "Status")]
        public bool? StatusFilter { get; set; }

        [Display(Name = "Page Size")]
        public int PageSize { get; set; } = 10;

        public int Page { get; set; } = 1;
    }
}
