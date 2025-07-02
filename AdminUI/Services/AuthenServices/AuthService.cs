using AdminUI.Models;
using AdminUI.Models.Authentication;
using System.Text;
using System.Text.Json;

namespace AdminUI.Services.AuthenServices
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
        }

        public async Task<LoginResponseModel> LoginAsync(LoginViewModel model)
        {
            try
            {
                _logger.LogInformation($"Attempting login for user: {model.Email}");
                _logger.LogInformation($"HttpClient BaseAddress: {_httpClient.BaseAddress}");

                if (_httpClient.BaseAddress == null)
                {
                    throw new InvalidOperationException("HttpClient BaseAddress is not set.");
                }

                var loginData = new
                {
                    email = model.Email,
                    password = model.Password
                };

                var jsonContent = JsonSerializer.Serialize(loginData);
                _logger.LogInformation($"Request JSON: {jsonContent}");

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Ensure BaseAddress is not null before constructing the full URL
                var fullUrl = new Uri(_httpClient.BaseAddress, "/gateway/users/login");
                _logger.LogInformation($"Full request URL: {fullUrl}");

                // Gọi API qua Gateway
                var response = await _httpClient.PostAsync("/gateway/users/login", content);

                _logger.LogInformation($"API Response Status: {response.StatusCode}");
                _logger.LogInformation($"Response Headers: {response.Headers}");

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"API Response Content: {responseContent}");

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

                    // Cố gắng parse error response
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
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "HTTP request error during login");
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "Unable to connect to the server. Please check your connection."
                };
            }
            catch (TaskCanceledException tcEx)
            {
                _logger.LogError(tcEx, "Request timeout during login");
                return new LoginResponseModel
                {
                    Success = false,
                    Message = "Request timeout. Please try again."
                };
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
            return session.GetString(TokenKey);
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            return !string.IsNullOrEmpty(token);
        }
    }
}