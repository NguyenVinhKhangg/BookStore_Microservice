using AdminUI.Models.Profile;

namespace AdminUI.Services.UserServices
{
    public interface IUserService
    {
        Task<ProfileResponseModel> GetProfileAsync();
        Task<UpdateProfileResponseModel> UpdateProfileAsync(UpdateProfileViewModel model);
        Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}
