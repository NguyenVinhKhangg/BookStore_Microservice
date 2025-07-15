using BookClient.Models.Authentication;

namespace BookClient.Services.AuthServices
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginViewModel model);
        Task<RegisterResponseModel> RegisterAsync(RegisterViewModel model);
        Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetPasswordViewModel model);
        Task<bool> LogoutAsync();
        Task<string> GetCurrentUserTokenAsync();
        Task<bool> IsAuthenticatedAsync();
    }
}