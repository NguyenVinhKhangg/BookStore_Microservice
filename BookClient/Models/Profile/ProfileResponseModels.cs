namespace BookClient.Models.Profile
{
    public class ProfileResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProfileViewModel Data { get; set; }
    }

    public class UpdateProfileResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ProfileViewModel Data { get; set; }
    }

    public class ChangePasswordResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}