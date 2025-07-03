using AdminUI.Models;
using AdminUI.Models.Authentication;

namespace AdminUI.Services.AuthenServices
{
    public interface IAuthService
    {
        Task<LoginResponseModel> LoginAsync(LoginViewModel model);
        Task<bool> LogoutAsync();
        Task<string> GetCurrentUserTokenAsync();
        Task<bool> IsAuthenticatedAsync();
        Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ForgotPasswordViewModel model);
        Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetPasswordViewModel model);
    }
}
