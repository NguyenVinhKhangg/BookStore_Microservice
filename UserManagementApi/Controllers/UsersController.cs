using BussinessObject.DTO.UserDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Security.Claims;
using UserManagementApi.DTOs;
using UserManagementApi.Services.Interface;

namespace UserManagementApi.Controllers
{
   /* [ApiController]
    [Route("api/[controller]")]*/
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        //[Authorize(Roles = "Admin,Staff")]
        [EnableQuery]  
        [HttpGet("/odata/users")]
        public IQueryable<UserDTO> GetUsersOData()
        {
            return _userService.GetUsersQueryable();
        }

        #region Authentication Endpoints (No Auth Required)

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var user = await _userService.RegisterUserAsync(registerDTO);
                return Ok(new
                {
                    success = true,
                    message = "User registered successfully",
                    data = user
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPasswordDTO)
        {
            try
            {
                await _userService.ForgotPasswordAsync(forgotPasswordDTO);
                return Ok(new
                {
                    success = true,
                    message = "OTP sent to your email"
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

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                await _userService.ResetPasswordAsync(resetPasswordDTO);
                return Ok(new
                {
                    success = true,
                    message = "Password reset successfully"
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

        #endregion

        #region User Profile Endpoints (Auth Required)

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(new
                {
                    success = true,
                    data = user
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

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateProfileDTO)
        {
            try
            {
                var userId = GetCurrentUserId();
                var updatedUser = await _userService.UpdateProfileAsync(userId, updateProfileDTO);
                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully",
                    data = updatedUser
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

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userId = GetCurrentUserId();
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

        #endregion

        #region Admin Only Endpoints

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/{id}")]
        public async Task<IActionResult> GetUserByIdAdmin(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAdminAsync(id);
                return Ok(new
                {
                    success = true,
                    data = user
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

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/users")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO createUserDTO)
        {
            try
            {
                var user = await _userService.CreateUserAsync(createUserDTO);
                return Ok(new
                {
                    success = true,
                    message = "User created successfully",
                    data = user
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

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/users/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDTO updateUserDTO)
        {
            try
            {
                var updatedUser = await _userService.UpdateUserAsync(id, updateUserDTO);
                return Ok(new
                {
                    success = true,
                    message = "User updated successfully",
                    data = updatedUser
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

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(int id)
        {
            try
            {
                await _userService.DeactivateUserAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "User deactivated successfully"
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

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(int id)
        {
            try
            {
                await _userService.ActivateUserAsync(id);
                return Ok(new
                {
                    success = true,
                    message = "User activated successfully"
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
      

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/total")]
        public async Task<IActionResult> GetTotalUsers()
        {
            try
            {
                var total = await _userService.GetTotalUserAsync();
                return Ok(new
                {
                    success = true,
                    data = new { totalUsers = total }
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

        #endregion


        #region Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid token or user ID not found");
            }
            return userId;
        }

        #endregion
    }
}