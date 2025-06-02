using AutoMapper;
using BussinessObject.DTO.UserDTO;
using System.Security.Claims;
using UserManagementApi.DTOs;
using UserManagementApi.Models;
using UserManagementApi.Repositories.Interface;
using UserManagementApi.Services.Interface;
using UserManagementApi.Utilities;

namespace UserManagementApi.Services.Implement
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly Dictionary<string, (string otp, DateTime expiry)> _otpStorage;

        public UserService(IConfiguration configuration,IUserRepository userRepository, IMapper mapper, IEmailService emailService, IJwtService jwtService)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _jwtService = jwtService;
            _otpStorage = new Dictionary<string, (string otp, DateTime expiry)>();
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAdminAsync()
        {
            var users = await _userRepository.GetAllUsersAdminAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByIdAdminAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAdminAsync(id);
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user != null ? _mapper.Map<UserDTO>(user) : null;
        }

        public async Task<UserDTO> CreateUserAsync(CreateUserDTO createUserDTO)
        {
            var user = _mapper.Map<Users>(createUserDTO);
            var createdUser = await _userRepository.AddUserAsync(user);
            return _mapper.Map<UserDTO>(createdUser);
        }

        public async Task<UserDTO> RegisterUserAsync(RegisterDTO registerDTO)
        {
            var registeredUser = await _userRepository.RegisterUserAsync(registerDTO);
            return _mapper.Map<UserDTO>(registeredUser);
        }

        public async Task<UserDTO> UpdateUserAsync(int id, UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAdminAsync(id);

            _mapper.Map(updateUserDTO, existingUser);

            bool updatePassword = !string.IsNullOrEmpty(updateUserDTO.Password);
            var updatedUser = await _userRepository.UpdateUserAsync(existingUser, updatePassword);

            return _mapper.Map<UserDTO>(updatedUser);
        }

        public async Task<UserDTO> UpdateProfileAsync(int id, UpdateProfileDTO updateProfileDTO)
        {
            var existingUser = await _userRepository.GetUserByIdAsync(id);

            _mapper.Map(updateProfileDTO, existingUser);
            existingUser.UpdatedAt = DateTime.Now;

            var updatedUser = await _userRepository.UpdateUserAsync(existingUser);
            return _mapper.Map<UserDTO>(updatedUser);
        }

        public async Task<bool> DeactivateUserAsync(int id)
        {
            return await _userRepository.DeactivateUserAsync(id);
        }

        public async Task<bool> ActivateUserAsync(int id)
        {
            return await _userRepository.ActivateUserAsync(id);
        }

        public async Task<IEnumerable<UserDTO>> SearchUsersByNameAsync(string name)
        {
            var users = await _userRepository.SearchUsersByNameAsync(name);
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<IEnumerable<UserDTO>> SearchUsersByNameAdminAsync(string name)
        {
            var users = await _userRepository.SearchUsersByNameAdminAsync(name);
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<LoginResponseDTO> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userRepository.VerifyLoginAsync(loginDTO.Email, loginDTO.Password);
            var userDTO = _mapper.Map<UserDTO>(user);

            // Generate JWT token
            var token = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            return new LoginResponseDTO
            {
                User = userDTO,
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "50"))
            };
        }

        public async Task<LoginResponseDTO> RefreshTokenAsync(RefreshTokenRequestDTO refreshTokenRequest)
        {
            var principal = _jwtService.GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new Exception("Invalid token");
            }

            var user = await _userRepository.GetUserByIdAsync(userId);

            // In production, you should validate the refresh token against stored refresh tokens
            // For now, we'll just generate new tokens

            var newToken = _jwtService.GenerateToken(user);
            var newRefreshToken = _jwtService.GenerateRefreshToken();
            var userDTO = _mapper.Map<UserDTO>(user);

            return new LoginResponseDTO
            {
                User = userDTO,
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.Now.AddMinutes(int.Parse(_configuration["Jwt:ExpireMinutes"] ?? "5"))
            };
        }
        public async Task<int> GetTotalUserAsync()
        {
            return await _userRepository.GetTotalUserAsync();
        }

        public async Task<PageList<UserDTO>> GetUsersWithPagingAdminAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name")
        {
            var usersPageList = await _userRepository.GetUsersWithPagingAdminAsync(pageNumber, pageSize, searchTerm, searchField);
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(usersPageList.Items);

            return new PageList<UserDTO>(userDTOs.ToList(), usersPageList.TotalCount, pageNumber, pageSize);
        }

        public async Task<PageList<UserDTO>> GetUsersWithPagingStaffAsync(int pageNumber, int pageSize, string searchTerm = null, string searchField = "name")
        {
            var usersPageList = await _userRepository.GetUsersWithPagingStaffAsync(pageNumber, pageSize, searchTerm, searchField);
            var userDTOs = _mapper.Map<IEnumerable<UserDTO>>(usersPageList.Items);

            return new PageList<UserDTO>(userDTOs.ToList(), usersPageList.TotalCount, pageNumber, pageSize);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            // Verify current password
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDTO.OldPassword, user.Password))
            {
                throw new Exception("Current password is incorrect.");
            }

            // Update with new password
            user.Password = changePasswordDTO.NewPassword; // Will be hashed in repository
            await _userRepository.UpdateUserAsync(user, true);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDTO forgotPasswordDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(forgotPasswordDTO.Email);
            if (user == null)
            {
                throw new Exception("Email not found.");
            }

            var otp = GenerateOTP();
            var expiryTime = DateTime.Now.AddMinutes(2);

            // Store OTP in memory (in production, consider using Redis or database)
            _otpStorage[forgotPasswordDTO.Email] = (otp, expiryTime);

            // Send OTP email
            await _emailService.SendOTPEmailAsync(forgotPasswordDTO.Email, otp);
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _userRepository.GetUserByEmailAsync(resetPasswordDTO.Email);
            if (user == null)
            {
                throw new Exception("Email not found.");
            }

            // Verify OTP
            if (!_otpStorage.ContainsKey(resetPasswordDTO.Email))
            {
                throw new Exception("OTP not found or expired.");
            }

            var (storedOtp, expiry) = _otpStorage[resetPasswordDTO.Email];

            if (DateTime.Now > expiry)
            {
                _otpStorage.Remove(resetPasswordDTO.Email);
                throw new Exception("OTP has expired.");
            }

            if (storedOtp != resetPasswordDTO.OTP)
            {
                throw new Exception("Invalid OTP.");
            }

            // Update password
            user.Password = resetPasswordDTO.NewPassword; // Will be hashed in repository
            await _userRepository.UpdateUserAsync(user, true);

            // Remove used OTP
            _otpStorage.Remove(resetPasswordDTO.Email);
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}