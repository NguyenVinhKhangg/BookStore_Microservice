using AdminUI.Models.Profile;
using AdminUI.Models.User;

namespace AdminUI.Services.UserServices
{
    public interface IUserService
    {
        Task<ProfileResponseModel> GetProfileAsync();
        Task<UpdateProfileResponseModel> UpdateProfileAsync(UpdateProfileViewModel model);
        Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordViewModel model);
        Task<UsersListResponseModel> GetUsersAsync(UserSearchFilterViewModel filter);
        Task<UserResponseModel> GetUserByIdAsync(int id);
        Task<UserResponseModel> CreateUserAsync(CreateUserViewModel model);
        Task<UserResponseModel> UpdateUserAsync(UpdateUserViewModel model);
        Task<UserResponseModel> ActivateUserAsync(int id);
        Task<UserResponseModel> DeactivateUserAsync(int id);
    }
}
