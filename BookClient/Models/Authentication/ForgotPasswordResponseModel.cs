namespace BookClient.Models.Authentication
{
    public class ForgotPasswordResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}