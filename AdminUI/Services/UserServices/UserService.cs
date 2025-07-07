using AdminUI.Models.Profile;
using AdminUI.Models.User;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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

        public async Task<UsersListResponseModel> GetUsersAsync(UserSearchFilterViewModel filter)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                // Build OData query
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    queryParams.Add($"$filter=contains(tolower(fullname),'{filter.SearchTerm.ToLower()}') or contains(tolower(email),'{filter.SearchTerm.ToLower()}')");
                }

                if (filter.RoleFilter.HasValue)
                {
                    var roleFilter = filter.RoleFilter.HasValue ? $"roleId eq {filter.RoleFilter.Value}" : "";
                    if (!string.IsNullOrEmpty(roleFilter))
                    {
                        if (queryParams.Any(q => q.StartsWith("$filter")))
                        {
                            queryParams[0] += $" and {roleFilter}";
                        }
                        else
                        {
                            queryParams.Add($"$filter={roleFilter}");
                        }
                    }
                }

                if (filter.StatusFilter.HasValue)
                {
                    var statusFilter = $"isDeactivated eq {filter.StatusFilter.Value.ToString().ToLower()}";
                    if (queryParams.Any(q => q.StartsWith("$filter")))
                    {
                        queryParams[0] += $" and {statusFilter}";
                    }
                    else
                    {
                        queryParams.Add($"$filter={statusFilter}");
                    }
                }

                queryParams.Add($"$skip={((filter.Page - 1) * filter.PageSize)}");
                queryParams.Add($"$top={filter.PageSize}");
                queryParams.Add("$count=true");
                queryParams.Add("$orderby=userID desc");

                var queryString = string.Join("&", queryParams);
                var url = $"/gateway/odata/users?{queryString}";

                _logger.LogInformation($"Calling OData URL: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"OData Response Status: {response.StatusCode}");
                _logger.LogInformation($"OData Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // ✅ Kiểm tra xem response có phải là OData format hay array thông thường
                        if (responseContent.TrimStart().StartsWith("{") && responseContent.Contains("@odata"))
                        {
                            // ✅ OData response format
                            var odataResponse = JsonSerializer.Deserialize<StandardODataResponse<UserManagementViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            return new UsersListResponseModel
                            {
                                Success = true,
                                Data = odataResponse.Value,
                                TotalCount = odataResponse.OdataCount ?? odataResponse.Value?.Count() ?? 0
                            };
                        }
                        else
                        {
                            // ✅ Simple array response format
                            var usersList = JsonSerializer.Deserialize<List<UserManagementViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });

                            return new UsersListResponseModel
                            {
                                Success = true,
                                Data = usersList,
                                TotalCount = usersList?.Count ?? 0
                            };
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, $"JSON Deserialization error. Response content: {responseContent}");
                        return new UsersListResponseModel
                        {
                            Success = false,
                            Message = $"Failed to parse response: {jsonEx.Message}"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning($"OData request failed with status: {response.StatusCode}, Content: {responseContent}");
                    return new UsersListResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get users: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users");
                return new UsersListResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving users."
                };
            }
        }

        public async Task<UserResponseModel> GetUserByIdAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync($"/gateway/users/admin/users/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var userResponse = JsonSerializer.Deserialize<UserResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return userResponse ?? new UserResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new UserResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get user: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by id");
                return new UserResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving user."
                };
            }
        }

        public async Task<UserResponseModel> CreateUserAsync(CreateUserViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var createData = new
                {
                    fullName = model.FullName,
                    email = model.Email,
                    password = model.Password,
                    phonenumber = model.Phonenumber,
                    address = model.Address,
                    birthDay = model.BirthDay,
                    roleId = model.RoleId
                };

                var jsonContent = JsonSerializer.Serialize(createData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/gateway/users/admin/users", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var userResponse = JsonSerializer.Deserialize<UserResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return userResponse ?? new UserResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    return new UserResponseModel
                    {
                        Success = false,
                        Message = $"Failed to create user: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return new UserResponseModel
                {
                    Success = false,
                    Message = "An error occurred while creating user."
                };
            }
        }

        public async Task<UserResponseModel> UpdateUserAsync(UpdateUserViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                // ✅ FIXED: Create object conditionally
                var updateData = new
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phonenumber = model.Phonenumber,
                    Address = model.Address,
                    BirthDay = model.BirthDay,
                    RoleId = model.RoleId,
                    // ✅ Only include password if provided
                    Password = string.IsNullOrWhiteSpace(model.Password) ? (string?)null : model.Password
                };

                var jsonContent = JsonSerializer.Serialize(updateData, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull // ✅ Ignore null values
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // ✅ ADDED: Debug logging
                _logger.LogInformation($"Updating user {model.UserID} with data: {jsonContent}");

                var response = await _httpClient.PutAsync($"/gateway/users/admin/users/{model.UserID}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // ✅ ADDED: Always log response details
                _logger.LogInformation($"Update user API response: Status={response.StatusCode}, Content={responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var userResponse = JsonSerializer.Deserialize<UserResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return userResponse ?? new UserResponseModel
                    {
                        Success = false,
                        Message = "Invalid response from server"
                    };
                }
                else
                {
                    // ✅ IMPROVED: Include response content in error message
                    return new UserResponseModel
                    {
                        Success = false,
                        Message = $"Failed to update user: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                return new UserResponseModel
                {
                    Success = false,
                    Message = "An error occurred while updating user."
                };
            }
        }

        public async Task<UserResponseModel> ActivateUserAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.PatchAsync($"/gateway/users/admin/users/{id}/activate", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Activate user API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    // ✅ Parse simple response format
                    var simpleResponse = JsonSerializer.Deserialize<SimpleResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new UserResponseModel
                    {
                        Success = simpleResponse?.Success ?? true,
                        Message = simpleResponse?.Message ?? "User activated successfully"
                    };
                }
                else
                {
                    // ✅ Try to parse error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<SimpleResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new UserResponseModel
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Failed to activate user: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new UserResponseModel
                        {
                            Success = false,
                            Message = $"Failed to activate user: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                return new UserResponseModel
                {
                    Success = false,
                    Message = "An error occurred while activating user."
                };
            }
        }

        public async Task<UserResponseModel> DeactivateUserAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.PatchAsync($"/gateway/users/admin/users/{id}/deactivate", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Deactivate user API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    // ✅ Parse simple response format
                    var simpleResponse = JsonSerializer.Deserialize<SimpleResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return new UserResponseModel
                    {
                        Success = simpleResponse?.Success ?? true,
                        Message = simpleResponse?.Message ?? "User deactivated successfully"
                    };
                }
                else
                {
                    // ✅ Try to parse error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<SimpleResponseModel>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new UserResponseModel
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Failed to deactivate user: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new UserResponseModel
                        {
                            Success = false,
                            Message = $"Failed to deactivate user: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                return new UserResponseModel
                {
                    Success = false,
                    Message = "An error occurred while deactivating user."
                };
            }
        }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; }
        public int Count { get; set; }
    }

    public class StandardODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; }

        [JsonPropertyName("@odata.count")]
        public int? OdataCount { get; set; }

        [JsonPropertyName("@odata.context")]
        public string OdataContext { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string OdataNextLink { get; set; }
    }
}