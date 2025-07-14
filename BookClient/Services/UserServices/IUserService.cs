using BookClient.Models.Profile;

namespace BookClient.Services.UserServices
{
    public interface IUserService
    {
        Task<ProfileResponseModel> GetProfileAsync();
        Task<UpdateProfileResponseModel> UpdateProfileAsync(UpdateProfileViewModel model);
        Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordViewModel model);
    }
}