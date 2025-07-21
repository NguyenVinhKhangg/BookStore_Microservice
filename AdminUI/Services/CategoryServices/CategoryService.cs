using AdminUI.Models.Category;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdminUI.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CategoryService> _logger;
        private const string TokenKey = "auth_token";

        public CategoryService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, ILogger<CategoryService> logger)
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

        public async Task<CategoriesListResponseModel> GetCategoriesAsync(CategorySearchFilterViewModel filter)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                _logger.LogInformation($"=== GETTING CATEGORIES WITH PAGINATION ===");
                _logger.LogInformation($"Filter: SearchTerm={filter.SearchTerm}, StatusFilter={filter.StatusFilter}");
                _logger.LogInformation($"Pagination: Page={filter.Page}, PageSize={filter.PageSize}");

                // ✅ STEP 1: Get total count from dedicated endpoint
                var totalCount = await GetTotalCategoriesCountAsync();
                _logger.LogInformation($"Total categories count from API: {totalCount}");

                // ✅ STEP 2: Build OData query for paginated data
                var queryParams = new List<string>();
                var filterConditions = new List<string>();

                // Add search filter
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    filterConditions.Add($"(contains(tolower(name),'{searchTerm}') or contains(tolower(description),'{searchTerm}'))");
                }

                // Add status filter
                if (filter.StatusFilter.HasValue)
                {
                    filterConditions.Add($"isActive eq {filter.StatusFilter.Value.ToString().ToLower()}");
                }

                // ✅ Combine all filter conditions
                if (filterConditions.Any())
                {
                    queryParams.Add($"$filter={string.Join(" and ", filterConditions)}");
                }

                // ✅ Add pagination parameters
                queryParams.Add($"$skip={((filter.Page - 1) * filter.PageSize)}");
                queryParams.Add($"$top={filter.PageSize}");
                queryParams.Add("$orderby=categoryID desc");

                var queryString = string.Join("&", queryParams);
                var url = $"/gateway/odata/categories?{queryString}";

                _logger.LogInformation($"Calling OData URL: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"OData Response Status: {response.StatusCode}");
                _logger.LogInformation($"OData Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // ✅ Parse OData response
                        List<CategoryViewModel> categories = null;

                        // Try OData format first
                        if (responseContent.TrimStart().StartsWith("{") && responseContent.Contains("value"))
                        {
                            var odataResponse = JsonSerializer.Deserialize<StandardODataResponse<CategoryViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            });
                            categories = odataResponse?.Value ?? new List<CategoryViewModel>();
                        }
                        else
                        {
                            // Try simple array format
                            categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(responseContent, new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<CategoryViewModel>();
                        }

                        _logger.LogInformation($"Parsed {categories.Count} categories from response");

                        // ✅ If filters are applied, we need to get filtered count
                        var effectiveTotalCount = totalCount;
                        if (filterConditions.Any())
                        {
                            // For filtered results, use the total count from a separate count query
                            effectiveTotalCount = await GetFilteredCategoriesCountAsync(filter);
                            _logger.LogInformation($"Filtered total count: {effectiveTotalCount}");
                        }

                        return new CategoriesListResponseModel
                        {
                            Success = true,
                            Data = categories,
                            TotalCount = effectiveTotalCount
                        };
                    }
                    catch (JsonException jsonEx)
                    {
                        _logger.LogError(jsonEx, $"JSON Deserialization error. Response content: {responseContent}");
                        return new CategoriesListResponseModel
                        {
                            Success = false,
                            Message = $"Failed to parse response: {jsonEx.Message}"
                        };
                    }
                }
                else
                {
                    _logger.LogWarning($"OData request failed with status: {response.StatusCode}, Content: {responseContent}");
                    return new CategoriesListResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get categories: {response.StatusCode} - {responseContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting categories");
                return new CategoriesListResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving categories."
                };
            }
        }

        // ✅ NEW: Get total categories count from dedicated endpoint
        private async Task<int> GetTotalCategoriesCountAsync()
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync("/gateway/categories/total");
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Total count API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var totalResponse = JsonSerializer.Deserialize<TotalCategoriesResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return totalResponse?.Data?.TotalCategories ?? 0;
                }
                else
                {
                    _logger.LogWarning($"Failed to get total count: {response.StatusCode}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total categories count");
                return 0;
            }
        }

        // ✅ NEW: Get filtered categories count 
        private async Task<int> GetFilteredCategoriesCountAsync(CategorySearchFilterViewModel filter)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                // Build count query with same filters but only get count
                var queryParams = new List<string>();
                var filterConditions = new List<string>();

                // Add search filter
                if (!string.IsNullOrEmpty(filter.SearchTerm))
                {
                    var searchTerm = filter.SearchTerm.ToLower();
                    filterConditions.Add($"(contains(tolower(name),'{searchTerm}') or contains(tolower(description),'{searchTerm}'))");
                }

                // Add status filter
                if (filter.StatusFilter.HasValue)
                {
                    filterConditions.Add($"isActive eq {filter.StatusFilter.Value.ToString().ToLower()}");
                }

                // ✅ Only get count
                if (filterConditions.Any())
                {
                    queryParams.Add($"$filter={string.Join(" and ", filterConditions)}");
                }
                queryParams.Add("$count=true");
                queryParams.Add("$top=0"); // Don't return any data, just count

                var queryString = string.Join("&", queryParams);
                var url = $"/gateway/odata/categories?{queryString}";

                _logger.LogInformation($"Getting filtered count from: {url}");

                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Try to extract count from OData response
                    if (responseContent.Contains("@odata.count"))
                    {
                        var odataResponse = JsonSerializer.Deserialize<StandardODataResponse<CategoryViewModel>>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return odataResponse?.OdataCount ?? 0;
                    }
                    else
                    {
                        // Fallback: parse as array and get length
                        var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        return categories?.Count ?? 0;
                    }
                }
                else
                {
                    _logger.LogWarning($"Failed to get filtered count: {response.StatusCode}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting filtered categories count");
                return 0;
            }
        }

        // Update GetCategoryByIdAsync method
        public async Task<CategoryResponseModel> GetCategoryByIdAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.GetAsync($"/gateway/categories/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"GetCategoryById API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    // ✅ Parse the API response format: { success, data, message }
                    var apiResponse = JsonSerializer.Deserialize<CategoryResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true && apiResponse.Data != null)
                    {
                        return new CategoryResponseModel
                        {
                            Success = true,
                            Data = new CategoryViewModel
                            {
                                CategoryID = apiResponse.Data.CategoryID,
                                Name = apiResponse.Data.Name,
                                Description = apiResponse.Data.Description,
                                IsActive = apiResponse.Data.IsActive
                            }
                        };
                    }
                    else
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = apiResponse?.Message ?? "Invalid response from server"
                        };
                    }
                }
                else
                {
                    // Try to parse error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Failed to get category: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = $"Failed to get category: {response.StatusCode}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving category."
                };
            }
        }

        // ✅ Update CreateCategoryAsync method
        public async Task<CategoryResponseModel> CreateCategoryAsync(CreateCategoryViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var createData = new
                {
                    name = model.Name,
                    description = model.Description
                };

                var jsonContent = JsonSerializer.Serialize(createData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Creating category with data: {jsonContent}");

                var response = await _httpClient.PostAsync("/gateway/categories", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Create category API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<CategoryResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true)
                    {
                        return new CategoryResponseModel
                        {
                            Success = true,
                            Message = apiResponse.Message ?? "Category created successfully",
                            Data = apiResponse.Data != null ? new CategoryViewModel
                            {
                                CategoryID = apiResponse.Data.CategoryID,
                                Name = apiResponse.Data.Name,
                                Description = apiResponse.Data.Description,
                                IsActive = apiResponse.Data.IsActive
                            } : null
                        };
                    }
                    else
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = apiResponse?.Message ?? "Failed to create category"
                        };
                    }
                }
                else
                {
                    // Try to parse error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Failed to create category: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = $"Failed to create category: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while creating category."
                };
            }
        }

        // ✅ Update UpdateCategoryAsync method
        public async Task<CategoryResponseModel> UpdateCategoryAsync(UpdateCategoryViewModel model)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var updateData = new
                {
                    name = model.Name,
                    description = model.Description,
                    isActive = model.IsActive
                };

                var jsonContent = JsonSerializer.Serialize(updateData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Updating category {model.CategoryID} with data: {jsonContent}");

                var response = await _httpClient.PutAsync($"/gateway/categories/{model.CategoryID}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Update category API response: {response.StatusCode}, Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = JsonSerializer.Deserialize<CategoryResponseModel>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (apiResponse?.Success == true)
                    {
                        return new CategoryResponseModel
                        {
                            Success = true,
                            Message = apiResponse.Message ?? "Category updated successfully",
                            Data = apiResponse.Data != null ? new CategoryViewModel
                            {
                                CategoryID = apiResponse.Data.CategoryID,
                                Name = apiResponse.Data.Name,
                                Description = apiResponse.Data.Description,
                                IsActive = apiResponse.Data.IsActive
                            } : null
                        };
                    }
                    else
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = apiResponse?.Message ?? "Failed to update category"
                        };
                    }
                }
                else
                {
                    // Try to parse error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = errorResponse?.Message ?? $"Failed to update category: {response.StatusCode}"
                        };
                    }
                    catch
                    {
                        return new CategoryResponseModel
                        {
                            Success = false,
                            Message = $"Failed to update category: {response.StatusCode} - {responseContent}"
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while updating category."
                };
            }
        }

        public async Task<CategoryResponseModel> DeleteCategoryAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.DeleteAsync($"/gateway/categories/{id}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new CategoryResponseModel
                    {
                        Success = true,
                        Message = "Category deleted successfully"
                    };
                }
                else
                {
                    return new CategoryResponseModel
                    {
                        Success = false,
                        Message = $"Failed to delete category: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while deleting category."
                };
            }
        }

        public async Task<CategoryResponseModel> ActivateCategoryAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.PatchAsync($"/gateway/categories/{id}/activate", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new CategoryResponseModel
                    {
                        Success = true,
                        Message = "Category activated successfully"
                    };
                }
                else
                {
                    return new CategoryResponseModel
                    {
                        Success = false,
                        Message = $"Failed to activate category: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating category");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while activating category."
                };
            }
        }

        public async Task<CategoryResponseModel> DeactivateCategoryAsync(int id)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                var response = await _httpClient.PatchAsync($"/gateway/categories/{id}/deactivate", null);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return new CategoryResponseModel
                    {
                        Success = true,
                        Message = "Category deactivated successfully"
                    };
                }
                else
                {
                    return new CategoryResponseModel
                    {
                        Success = false,
                        Message = $"Failed to deactivate category: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating category");
                return new CategoryResponseModel
                {
                    Success = false,
                    Message = "An error occurred while deactivating category."
                };
            }
        }
    }

    public class StandardODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new List<T>();

        [JsonPropertyName("@odata.count")]
        public int? OdataCount { get; set; }

        [JsonPropertyName("@odata.context")]
        public string? OdataContext { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string? OdataNextLink { get; set; }
    }

    // ✅ Response model for total categories count
    public class TotalCategoriesResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("data")]
        public TotalCategoriesData? Data { get; set; }
    }

    public class TotalCategoriesData
    {
        [JsonPropertyName("totalCategories")]
        public int TotalCategories { get; set; }
    }
}