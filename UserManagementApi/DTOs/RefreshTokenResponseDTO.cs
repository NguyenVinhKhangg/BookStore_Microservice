namespace UserManagementApi.DTOs
{
    public class RefreshTokenResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
