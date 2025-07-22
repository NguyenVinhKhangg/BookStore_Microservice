namespace AdminUI.Models.Authentication
{
    public class LoginDataModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserModel User { get; set; }
    }
}
