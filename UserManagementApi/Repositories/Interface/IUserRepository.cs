using BussinessObject.DTO.UserDTO;
using UserManagementApi.Models;
using UserManagementApi.Utilities;

namespace UserManagementApi.Repositories.Interface
{
    public interface IUserRepository
    {
        Task<IEnumerable<Users>> GetAllUsersAdminAsync();
        Task<IEnumerable<Users>> GetAllUsersAsync();
        Task<Users> GetUserByIdAsync(int id);
        Task<Users> GetUserByIdAdminAsync(int id);
        Task<Users> GetUserByEmailAsync(string email);
        Task<Users> AddUserAsync(Users user);
        Task<Users> RegisterUserAsync(RegisterDTO registerDTO);
        Task<Users> UpdateUserAsync(Users user, bool updatePassword = false);
        Task<bool> DeactivateUserAsync(int id);
        Task<bool> ActivateUserAsync(int id);
        Task<IEnumerable<Users>> SearchUsersByNameAsync(string name);
        Task<IEnumerable<Users>> SearchUsersByNameAdminAsync(string name);
        Task<Users> VerifyLoginAsync(string email, string password);
        Task<int> GetTotalUserAsync();
        Task<PageList<Users>> GetUsersWithPagingAdminAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name");
        Task<PageList<Users>> GetUsersWithPagingStaffAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name");
    }
}
