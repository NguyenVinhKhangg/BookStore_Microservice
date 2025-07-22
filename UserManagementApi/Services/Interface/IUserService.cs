using BussinessObject.DTO.UserDTO;
using UserManagementApi.DTOs;
using UserManagementApi.Utilities;

namespace UserManagementApi.Services.Interface
{
    public interface IUserService
    {
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> GetUserByIdAdminAsync(int id);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<UserDTO> RegisterUserAsync(RegisterDTO registerDTO);
        Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<UserDTO> UpdateProfileAsync(int id, UpdateProfileDTO updateProfileDTO);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<int> GetTotalUserAsync();
        Task ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);
        Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task<LoginResponseDTO> LoginAdminAsync(LoginDTO loginDTO);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequest);
        IQueryable<UserDTO> GetUsersQueryable();
    }
}
