namespace BookClient.Models.Authentication
{
    public class LoginDataModel
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public UserModel User { get; set; } = new UserModel();
    }
}