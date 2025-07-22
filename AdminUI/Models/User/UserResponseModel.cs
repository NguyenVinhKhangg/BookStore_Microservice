namespace AdminUI.Models.User
{
    public class UserResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserManagementViewModel Data { get; set; }
    }
}
