using BussinessObject.DTO.UserDTO;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Data;
using UserManagementApi.Models;
using UserManagementApi.Repositories.Interface;
using UserManagementApi.Utilities;

namespace UserManagementApi.Repositories.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;
        private const int USER_ROLE_ID = 2;
        private const int ADMIN_ROLE_ID = 1;
        private const int STAFF_ROLE_ID = 3;

        public UserRepository(UserDbContext context)
        {
            _context = context;
        }


        public async Task<Users> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserID == id && !u.DeactivatedStatus);

                if (user == null)
                {
                    throw new Exception($"User with ID {id} not found or is deactivated");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to get user", ex);
            }
        }

        public async Task<Users> GetUserByIdAdminAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserID == id);

                if (user == null)
                {
                    throw new Exception($"User with ID {id} not found");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to get user", ex);
            }
        }

        public async Task<Users> GetUserByEmailAsync(string email)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
                if (user == null || user.DeactivatedStatus)
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to get user by email", ex);
            }
        }

        public async Task<Users> AddUserAsync(Users user)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower());
                if (existingUser != null)
                {
                    throw new Exception("Email already exists");
                }

                if (!await _context.Roles.AnyAsync(r => r.RoleID == user.RoleID))
                {
                    throw new Exception($"Role with ID {user.RoleID} does not exist");
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                user.DeactivatedStatus = false;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to add user", ex);
            }
        }

        public async Task<Users> RegisterUserAsync(RegisterDTO registerDTO)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == registerDTO.Email.ToLower());
                if (existingUser != null)
                {
                    throw new Exception("Email already exists");
                }

                if (!await _context.Roles.AnyAsync(r => r.RoleID == USER_ROLE_ID))
                {
                    throw new Exception($"Default role with ID {USER_ROLE_ID} does not exist");
                }

                var newUser = new Users
                {
                    FullName = registerDTO.FullName,
                    Email = registerDTO.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerDTO.Password),
                    PhoneNumber = registerDTO.Phonenumber,
                    Address = registerDTO.Address,
                    RoleID = USER_ROLE_ID,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    DeactivatedStatus = false,
                    BirthDay = registerDTO.BirthDay
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
                return newUser;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }

        public async Task<Users> UpdateUserAsync(Users user, bool updatePassword = false)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == user.UserID);

                if (existingUser == null)
                {
                    throw new Exception($"User with ID {user.UserID} not found");
                }

                var duplicateEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == user.Email.ToLower() && u.UserID != user.UserID);
                if (duplicateEmail != null)
                {
                    throw new Exception("Email already exists");
                }

                if (!await _context.Roles.AnyAsync(r => r.RoleID == user.RoleID))
                {
                    throw new Exception($"Role with ID {user.RoleID} does not exist");
                }

                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;
                existingUser.RoleID = user.RoleID;
                existingUser.UpdatedAt = DateTime.Now;
                existingUser.DeactivatedStatus = user.DeactivatedStatus;
                existingUser.BirthDay = user.BirthDay;

                if (updatePassword)
                {
                    existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                }

                await _context.SaveChangesAsync();
                return existingUser;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to update user", ex);
            }
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == id);

                if (user == null)
                {
                    throw new Exception($"User with ID {id} not found");
                }

                if (user.DeactivatedStatus)
                {
                    throw new Exception($"User with ID {id} is already deactivated");
                }

                user.DeactivatedStatus = true;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to deactivate user", ex);
            }
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserID == id);

                if (user == null)
                {
                    throw new Exception($"User with ID {id} not found");
                }

                if (!user.DeactivatedStatus)
                {
                    throw new Exception($"User with ID {id} is already activated");
                }

                user.DeactivatedStatus = false;
                user.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to activate user", ex);
            }
        }

        public async Task<Users> VerifyLoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if (user.RoleID != USER_ROLE_ID)
                {
                    throw new Exception("Access denied! Only User can access this system.");
                }

                if (user != null && user.DeactivatedStatus)
                {
                    throw new Exception("Your account was deactivated! Please register a new account!");
                }

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    throw new Exception("Invalid email or password!");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw ;
            }
        }

        public async Task<Users> VerifyLoginAdminAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

                if(user.RoleID != ADMIN_ROLE_ID && user.RoleID != STAFF_ROLE_ID)
                {
                    throw new Exception("Access denied! Only Admin and Staff can access this system.");
                }

                if (user != null && user.DeactivatedStatus)
                {
                    throw new Exception("Your account was deactivated! Please register a new account!");
                }

                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    throw new Exception("Invalid email or password!");
                }

                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> GetTotalUserAsync()
        {
            try
            {
                return await _context.Users.Where(x => x.RoleID != ADMIN_ROLE_ID).CountAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Fail to get total users", ex);
            }
        }    

        public IQueryable<Users> GetUsersQueryable()
        {
            return _context.Users.Include(u => u.Role).Where(u => u.RoleID != ADMIN_ROLE_ID);
        }
    }
}