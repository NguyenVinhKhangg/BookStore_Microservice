namespace AdminUI.Models.Authentication
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int RoleId { get; set; }
    }
}
