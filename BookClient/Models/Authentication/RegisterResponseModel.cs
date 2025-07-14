namespace BookClient.Models.Authentication
{
    public class RegisterResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserModel Data { get; set; } = new UserModel();
    }
}