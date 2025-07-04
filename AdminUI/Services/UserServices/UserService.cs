using AdminUI.Models.Profile;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AdminUI.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private const string TokenKey = "auth_token";

        public UserService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        private async Task<string> GetCurrentUserTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(TokenKey) ?? string.Empty;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<ProfileResponseModel> GetProfileAsync()
        {
            try
            {
                var token = await GetCurrentUserTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return new ProfileResponseModel
                    {
                        Success = false,
                        Message = "Authentication required"
                    };
                }

                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync("/gateway/users/profile");
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Get profile API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var profileResponse = JsonSerializer.Deserialize<ProfileResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return profileResponse ?? new ProfileResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new ProfileResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get profile: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during get profile process");
                return new ProfileResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving profile. Please try again."
                };
            }
        }

        public async Task<UpdateProfileResponseModel> UpdateProfileAsync(UpdateProfileViewModel model)
        {
            try
            {
                var token = await GetCurrentUserTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return new UpdateProfileResponseModel
                    {
                        Success = false,
                        Message = "Authentication required"
                    };
                }

                await SetAuthorizationHeaderAsync();

                var updateData = new
                {
                    fullName = model.FullName,
                    address = model.Address,
                    phonenumber = model.Phonenumber,
                    birthDay = model.BirthDay
                };

                var jsonContent = JsonSerializer.Serialize(updateData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync("/gateway/users/profile", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Update profile API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var updateResponse = JsonSerializer.Deserialize<UpdateProfileResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return updateResponse ?? new UpdateProfileResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new UpdateProfileResponseModel
                    {
                        Success = false,
                        Message = $"Failed to update profile: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during update profile process");
                return new UpdateProfileResponseModel
                {
                    Success = false,
                    Message = "An error occurred while updating profile. Please try again."
                };
            }
        }

        public async Task<ChangePasswordResponseModel> ChangePasswordAsync(ChangePasswordViewModel model)
        {
            try
            {
                var token = await GetCurrentUserTokenAsync();
                if (string.IsNullOrEmpty(token))
                {
                    return new ChangePasswordResponseModel
                    {
                        Success = false,
                        Message = "Authentication required"
                    };
                }

                await SetAuthorizationHeaderAsync();

                var changePasswordData = new
                {
                    oldPassword = model.OldPassword,
                    newPassword = model.NewPassword,
                    confirmNewPassword = model.ConfirmNewPassword
                };

                var jsonContent = JsonSerializer.Serialize(changePasswordData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/gateway/users/change-password", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Change password API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var changePasswordResponse = JsonSerializer.Deserialize<ChangePasswordResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return changePasswordResponse ?? new ChangePasswordResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new ChangePasswordResponseModel
                    {
                        Success = false,
                        Message = $"Failed to change password: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during change password process");
                return new ChangePasswordResponseModel
                {
                    Success = false,
                    Message = "An error occurred while changing password. Please try again."
                };
            }
        }
    }
}