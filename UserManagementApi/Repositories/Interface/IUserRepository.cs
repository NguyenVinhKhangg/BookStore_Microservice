using BussinessObject.DTO.UserDTO;
using UserManagementApi.Models;
using UserManagementApi.Utilities;

namespace UserManagementApi.Repositories.Interface
{
    public interface IUserRepository
    {
        IQueryable<Users> GetUsersQueryable();
        Task<Users> GetUserByIdAsync(int id);
        Task<Users> GetUserByIdAdminAsync(int id);
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> AddUserAsync(Users user);
        Task<Users> RegisterUserAsync(RegisterDTO registerDTO);
        Task<Users> UpdateUserAsync(Users user, bool updatePassword = false);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<Users> VerifyLoginAsync(string email, string password);
        Task<Users> VerifyLoginAdminAsync(string email, string password);
        Task<int> GetTotalUserAsync();
    }
}
