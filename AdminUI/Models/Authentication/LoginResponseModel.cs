namespace AdminUI.Models.Authentication
{
    public class LoginResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public LoginDataModel Data { get; set; }
    }
}
