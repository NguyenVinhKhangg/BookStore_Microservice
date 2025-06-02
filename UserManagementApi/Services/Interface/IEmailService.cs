namespace UserManagementApi.Services.Interface
{
    public interface IEmailService
    {
        Task SendOTPEmailAsync(string email, string otp);
    }
}

