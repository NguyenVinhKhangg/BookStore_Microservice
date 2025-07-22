namespace BookClient.Models.Authentication
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public LoginDataModel Data { get; set; } = new LoginDataModel();
    }
}