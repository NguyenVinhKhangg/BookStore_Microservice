using BussinessObject.DTO.UserDTO;
using UserManagementApi.DTOs;
using UserManagementApi.Utilities;

namespace UserManagementApi.Services.Interface
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAdminAsync();
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO> GetUserByIdAsync(int id);
        Task<UserDTO> GetUserByIdAdminAsync(int id);
        Task<UserDTO> GetUserByEmailAsync(string email);
        Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDTO);
        Task<UserDTO> RegisterUserAsync(RegisterDTO registerDTO);
        Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<UserDTO> UpdateProfileAsync(int id, UpdateProfileDTO updateProfileDTO);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<IEnumerable<UserDTO>> SearchUsersByNameAsync(string name);
        Task<IEnumerable<UserDTO>> SearchUsersByNameAdminAsync(string name);
        Task<int> GetTotalUserAsync();
        Task<PageList<UserDTO>> GetUsersWithPagingAdminAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name");
        Task<PageList<UserDTO>> GetUsersWithPagingStaffAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name");
        Task ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);
        Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO);
        Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO);
        Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO);
        Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequest);
    }
}
