namespace AdminUI.Models.Profile
{
    public class UpdateProfileResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProfileViewModel Data { get; set; }
    }
}
