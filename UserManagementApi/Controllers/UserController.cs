using BussinessObject.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementApi.DTOs;
using UserManagementApi.Services.Interface;

namespace UserManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User/admin/all
        [HttpGet("admin/all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsersAdmin()
        {
            try
            {
                var users = await _userService.GetAllUsersAdminAsync();
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/all
        [HttpGet("all")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/admin/{id}
        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserByIdAdmin(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAdminAsync(id);
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/email/{email}
        [HttpGet("email/{email}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    return NotFound(new { success = false, message = "User not found" });
                }
                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // POST: api/User/create
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var user = await _userService.CreateUserAsync(createUserDTO);
                return Created($"api/User/{user.UserID}", new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // POST: api/User/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var user = await _userService.RegisterUserAsync(registerDTO);
                return Created($"api/User/{user.UserID}", new { success = true, data = user });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var response = await _userService.LoginAsync(loginDTO);
                return Ok(new
                {
                    success = true,
                    message = "Login successful",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO request)
        {
            try
            {
                var response = await _userService.RefreshTokenAsync(request);
                return Ok(new
                {
                    success = true,
                    message = "Token refreshed successfully",
                    data = response
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
        // PUT: api/User/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var user = await _userService.UpdateUserAsync(id, updateUserDTO);
                return Ok(new { success = true, data = user, message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT: api/User/profile/{id}
        [HttpPut("profile/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileDTO updateProfileDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                var user = await _userService.UpdateProfileAsync(id, updateProfileDTO);
                return Ok(new { success = true, data = user, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                // Get user ID from JWT token
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                if (userId == 0)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Invalid token"
                    });
                }

                await _userService.ChangePasswordAsync(userId, changePasswordDTO);

                return Ok(new
                {
                    success = true,
                    message = "Password changed successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // POST: api/User/forgot-password
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                await _userService.ForgotPasswordAsync(forgotPasswordDTO);
                return Ok(new { success = true, message = "OTP has been sent to your email" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // POST: api/User/reset-password
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { success = false, message = "Invalid input data", errors = ModelState });
                }

                await _userService.ResetPasswordAsync(resetPasswordDTO);
                return Ok(new { success = true, message = "Password reset successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT: api/User/deactivate/{id}
        [HttpPut("deactivate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                var result = await _userService.DeactivateUserAsync(id);
                return Ok(new { success = true, message = "User deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // PUT: api/User/activate/{id}
        [HttpPut("activate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                var result = await _userService.ActivateUserAsync(id);
                return Ok(new { success = true, message = "User activated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/search
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> SearchUsersByName([FromQuery] string name)
        {
            try
            {
                var users = await _userService.SearchUsersByNameAsync(name);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/admin/search
        [HttpGet("admin/search")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SearchUsersByNameAdmin([FromQuery] string name)
        {
            try
            {
                var users = await _userService.SearchUsersByNameAdminAsync(name);
                return Ok(new { success = true, data = users });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/total
        [HttpGet("total")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalUsers()
        {
            try
            {
                var total = await _userService.GetTotalUserAsync();
                return Ok(new { success = true, data = total });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/admin/paging
        [HttpGet("admin/paging")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersWithPagingAdmin([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null, [FromQuery] string searchField = "name")
        {
            try
            {
                var users = await _userService.GetUsersWithPagingAdminAsync(pageNumber, pageSize, searchTerm, searchField);
                return Ok(new
                {
                    success = true,
                    data = users.Items,
                    pagination = new
                    {
                        pageNumber = users.PageNumber,
                        pageSize = users.PageSize,
                        totalCount = users.TotalCount,
                        totalPages = users.TotalPages,
                        hasPrevious = users.HasPrevious,
                        hasNext = users.HasNext
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // GET: api/User/staff/paging
        [HttpGet("staff/paging")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetUsersWithPagingStaff([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchTerm = null, [FromQuery] string searchField = "name")
        {
            try
            {
                var users = await _userService.GetUsersWithPagingStaffAsync(pageNumber, pageSize, searchTerm, searchField);
                return Ok(new
                {
                    success = true,
                    data = users.Items,
                    pagination = new
                    {
                        pageNumber = users.PageNumber,
                        pageSize = users.PageSize,
                        totalCount = users.TotalCount,
                        totalPages = users.TotalPages,
                        hasPrevious = users.HasPrevious,
                        hasNext = users.HasNext
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}