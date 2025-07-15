using BookClient.Models.Authentication;
using System.Text;
using System.Text.Json;

namespace BookClient.Services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthService> _logger;
        private const string TokenKey = "auth_token";
        private const string RefreshTokenKey = "refresh_token";

        public AuthService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<AuthService> logger)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;

            // ✅ Log HttpClient configuration for debugging
            _logger.LogInformation($"AuthService initialized with HttpClient BaseAddress: {_httpClient.BaseAddress}");
        }

        public async Task<LoginResponseModel> LoginAsync(LoginViewModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting login for user: {model.Email}");

                // ✅ Log HttpClient BaseAddress before making request
                _logger.LogInformation($"HttpClient BaseAddress: {_httpClient.BaseAddress}");

                var loginData = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // ✅ Build the full URI manually if BaseAddress is null
                string requestUri;
                if (_httpClient.BaseAddress != null)
                {
                    requestUri = "/gateway/users/login";
                }
                else
                {
                    requestUri = "https://localhost:7000/gateway/users/login";
                    _logger.LogWarning("BaseAddress is null, using absolute URI: {RequestUri}", requestUri);
                }

                _logger.LogInformation($"Making request to: {requestUri}");

                // Gọi API qua Gateway - sử dụng endpoint cho user thông thường
                var response = await _httpClient.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Login API Response Status: {response.StatusCode}");
                _logger.LogInformation($"Response content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (loginResponse != null && loginResponse.Success)
                    {
                        // Lưu token vào session
                        var session = _httpContextAccessor.HttpContext.Session;
                        session.SetString(TokenKey, loginResponse.Data.Token);
                        session.SetString(RefreshTokenKey, loginResponse.Data.RefreshToken);

                        // Lưu thông tin user
                        session.SetString("UserInfo", JsonSerializer.Serialize(loginResponse.Data.User));

                        _logger.LogInformation($"User {model.Email} logged in successfully");
                    }

                    return loginResponse ?? new LoginResponseModel { Success = false, Message = "Invalid response from server" };
                }
                else
                {
                    _logger.LogWarning($"Login failed with status code: {response.StatusCode}");

                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<LoginResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return errorResponse ?? new LoginResponseModel
                        {
                            Success = false,
                            Message = $"Login failed with status: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new LoginResponseModel
                        {
                            Success = false,
                            Message = $"Login failed: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "An unexpected error occurred during login. Please try again."
                };
            }
        }

        public async Task<RegisterResponseModel> RegisterAsync(RegisterViewModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting registration for user: {model.Email}");

                var registerData = new
                {
                    fullName = model.FullName,
                    email = model.Email,
                    password = model.Password,
                    phonenumber = model.PhoneNumber,
                    address = model.Address,
                    birthDay = model.BirthDay
                };

                var jsonContent = JsonSerializer.Serialize(registerData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // ✅ Build the full URI manually if BaseAddress is null
                string requestUri;
                if (_httpClient.BaseAddress != null)
                {
                    requestUri = "/gateway/users/register";
                }
                else
                {
                    requestUri = "https://localhost:7000/gateway/users/register";
                    _logger.LogWarning("BaseAddress is null, using absolute URI: {RequestUri}", requestUri);
                }

                var response = await _httpClient.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Register API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var registerResponse = JsonSerializer.Deserialize<RegisterResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return registerResponse ?? new RegisterResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<RegisterResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return errorResponse ?? new RegisterResponseModel
                        {
                            Success = false,
                            Message = $"Registration failed: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new RegisterResponseModel
                        {
                            Success = false,
                            Message = $"Registration failed: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration process");
                return new RegisterResponseModel
                {
                    Success = false,
                    Message = "An unexpected error occurred during registration. Please try again."
                };
            }
        }

        public async Task<ForgotPasswordResponseModel> ForgotPasswordAsync(ForgotPasswordViewModel model)
        {
            try
            {
                _logger.LogInformation($"Sending forgot password request for email: {model.Email}");

                var forgotPasswordData = new
                {
                    email = model.Email
                };

                var jsonContent = JsonSerializer.Serialize(forgotPasswordData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // ✅ Build the full URI manually if BaseAddress is null
                string requestUri;
                if (_httpClient.BaseAddress != null)
                {
                    requestUri = "/gateway/users/forgot-password";
                }
                else
                {
                    requestUri = "https://localhost:7000/gateway/users/forgot-password";
                    _logger.LogWarning("BaseAddress is null, using absolute URI: {RequestUri}", requestUri);
                }

                var response = await _httpClient.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Forgot password API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var forgotPasswordResponse = JsonSerializer.Deserialize<ForgotPasswordResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    // Lưu email vào session để dùng cho reset password
                    if (forgotPasswordResponse?.Success == true)
                    {
                        var session = _httpContextAccessor.HttpContext.Session;
                        session.SetString("ResetEmail", model.Email);
                        session.SetString("OTPExpiration", DateTime.UtcNow.AddMinutes(5).ToString());
                    }

                    return forgotPasswordResponse ?? new ForgotPasswordResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ForgotPasswordResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return errorResponse ?? new ForgotPasswordResponseModel
                        {
                            Success = false,
                            Message = $"Request failed: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new ForgotPasswordResponseModel
                        {
                            Success = false,
                            Message = $"Request failed: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password process");
                return new ForgotPasswordResponseModel
                {
                    Success = false,
                    Message = "An error occurred while processing your request. Please try again."
                };
            }
        }

        public async Task<ResetPasswordResponseModel> ResetPasswordAsync(ResetPasswordViewModel model)
        {
            try
            {
                _logger.LogInformation($"Sending reset password request for email: {model.Email}");

                var resetPasswordData = new
                {
                    email = model.Email,
                    otp = model.OTP,
                    newPassword = model.NewPassword,
                    confirmNewPassword = model.ConfirmNewPassword
                };

                var jsonContent = JsonSerializer.Serialize(resetPasswordData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // ✅ Build the full URI manually if BaseAddress is null
                string requestUri;
                if (_httpClient.BaseAddress != null)
                {
                    requestUri = "/gateway/users/reset-password";
                }
                else
                {
                    requestUri = "https://localhost:7000/gateway/users/reset-password";
                    _logger.LogWarning("BaseAddress is null, using absolute URI: {RequestUri}", requestUri);
                }

                var response = await _httpClient.PostAsync(requestUri, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Reset password API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var resetPasswordResponse = JsonSerializer.Deserialize<ResetPasswordResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (resetPasswordResponse == null)
                    {
                        return new ResetPasswordResponseModel
                        {
                            Success = true,
                            Message = "Password reset successfully."
                        };
                    }

                    // Xóa session sau khi reset thành công
                    if (resetPasswordResponse.Success)
                    {
                        var session = _httpContextAccessor.HttpContext.Session;
                        session.Remove("ResetEmail");
                        session.Remove("OTPExpiration");
                    }

                    return resetPasswordResponse;
                }
                else
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ResetPasswordResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return errorResponse ?? new ResetPasswordResponseModel
                        {
                            Success = false,
                            Message = $"Reset failed: {response.StatusCode}"
                        };
                    }
                    catch (JsonException)
                    {
                        return new ResetPasswordResponseModel
                        {
                            Success = false,
                            Message = $"Reset failed: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during reset password process");
                return new ResetPasswordResponseModel
                {
                    Success = false,
                    Message = "An error occurred while resetting password. Please try again."
                };
            }
        }

        public async Task<bool> LogoutAsync()
        {
            try
            {
                var session = _httpContextAccessor.HttpContext.Session;
                session.Remove(TokenKey);
                session.Remove(RefreshTokenKey);
                session.Remove("UserInfo");
                session.Clear();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return false;
            }
        }

        public async Task<string> GetCurrentUserTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            return session?.GetString(TokenKey) ?? string.Empty;
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}