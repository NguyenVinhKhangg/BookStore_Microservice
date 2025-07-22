using AdminUI.Models.Book;
using AdminUI.Models.Category;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;

namespace AdminUI.Services.BookServices
{
    public class BookService : IBookService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly string _apiBaseUrl;
        private readonly ILogger<BookService> _logger;

        public BookService(HttpClient httpClient, IConfiguration configuration, ILogger<BookService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _apiBaseUrl = _configuration["ApiGateway:BaseUrl"];
            _logger = logger;
        }

        public async Task<(List<BookResponseModel> books, int totalCount)> GetAllAsync(
            string? searchTerm = null,
            int? categoryFilter = null,
            bool? isActiveFilter = null,
            string? sortBy = "Title",
            string? sortOrder = "asc",
            int page = 1,
            int pageSize = 10)
        {
            try
            {
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(searchTerm))
                    queryParams.Add($"searchTerm={Uri.EscapeDataString(searchTerm)}");

                if (categoryFilter.HasValue)
                    queryParams.Add($"categoryFilter={categoryFilter}");

                if (isActiveFilter.HasValue)
                    queryParams.Add($"isActiveFilter={isActiveFilter}");

                queryParams.Add($"sortBy={sortBy}");
                queryParams.Add($"sortOrder={sortOrder}");
                queryParams.Add($"page={page}");
                queryParams.Add($"pageSize={pageSize}");

                var queryString = string.Join("&", queryParams);

                // ✅ SỬA: AdminUI sử dụng admin endpoint để xem tất cả sách
                var url = $"{_apiBaseUrl}/gateway/books/admin/all?{queryString}";

                _logger.LogInformation($"Calling GetAllAsync: {url}");
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var books = await response.Content.ReadFromJsonAsync<List<BookResponseModel>>(options) ?? new List<BookResponseModel>();

                    return (books, books.Count);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"GetAllAsync failed with status: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"API call failed with status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllAsync");
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<BookResponseModel> GetByIdAsync(int id)
        {
            try
            {
                var url = $"{_apiBaseUrl}/gateway/books/admin/{id}/detail";

                _logger.LogInformation($"Calling GetByIdAsync: {url}");
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return await response.Content.ReadFromJsonAsync<BookResponseModel>(options);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"GetByIdAsync failed with status: {response.StatusCode}, Content: {errorContent}");
                throw new HttpRequestException($"API call failed with status: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByIdAsync for id: {Id}", id);
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<BookResponseModel> CreateAsync(CreateBookViewModel model)
        {
            try
            {
                // ✅ SỬA: Gọi qua Gateway
                var url = $"{_apiBaseUrl}/gateway/books";

                _logger.LogInformation($"Calling CreateAsync: {url}");
                _logger.LogInformation($"Model: {System.Text.Json.JsonSerializer.Serialize(model)}");

                var response = await _httpClient.PostAsJsonAsync(url, model);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"CreateAsync Response Status: {response.StatusCode}");
                _logger.LogInformation($"CreateAsync Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        var result = await response.Content.ReadFromJsonAsync<SimpleResponseModel>();
                        if (result?.Data != null)
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            return JsonSerializer.Deserialize<BookResponseModel>(result.Data.ToString(), options);
                        }
                        else
                        {
                            // Fallback: thử parse trực tiếp
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            return JsonSerializer.Deserialize<BookResponseModel>(responseContent, options);
                        }
                    }
                    catch (Exception parseEx)
                    {
                        _logger.LogError(parseEx, "Error parsing CreateAsync response: {Content}", responseContent);
                        throw new HttpRequestException($"Error parsing response: {parseEx.Message}");
                    }
                }

                _logger.LogError($"CreateAsync failed with status: {response.StatusCode}, Content: {responseContent}");
                throw new HttpRequestException($"API call failed with status: {response.StatusCode}. Content: {responseContent}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAsync");
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookViewModel model)
        {
            try
            {
                // ✅ SỬA: Gọi qua Gateway
                var url = $"{_apiBaseUrl}/gateway/books/{id}/update";

                _logger.LogInformation($"Calling UpdateAsync: {url}");
                var response = await _httpClient.PutAsJsonAsync(url, model);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"UpdateAsync failed with status: {response.StatusCode}, Content: {errorContent}");
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAsync for id: {Id}", id);
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<bool> HideAsync(int id)
        {
            try
            {
                // ✅ SỬA: Gọi qua Gateway
                var url = $"{_apiBaseUrl}/gateway/books/{id}/hideBook";

                _logger.LogInformation($"Calling HideAsync: {url}");
                var response = await _httpClient.DeleteAsync(url);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in HideAsync for id: {Id}", id);
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<bool> UnhideAsync(int id)
        {
            try
            {
                // ✅ SỬA: Gọi qua Gateway
                var url = $"{_apiBaseUrl}/gateway/books/{id}/unhide";

                _logger.LogInformation($"Calling UnhideAsync: {url}");
                var response = await _httpClient.PutAsync(url, null);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnhideAsync for id: {Id}", id);
                throw new HttpRequestException($"Error calling Book API: {ex.Message}", ex);
            }
        }

        public async Task<List<SelectListItem>> GetCategoryOptionsAsync()
        {
            try
            {
                var url = $"{_apiBaseUrl}/gateway/odata/categories?$filter=IsActive eq true";
                _logger.LogInformation($"Calling GetCategoryOptionsAsync: {url}");

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation($"GetCategoryOptionsAsync Response: {responseContent}");

                    try
                    {
                        // Parse OData response { "value": [...] }
                        var odataResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, options);
                        if (odataResponse.TryGetProperty("value", out var valueElement))
                        {
                            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(valueElement.GetRawText(), options) ?? new List<CategoryViewModel>();
                            return categories.Select(c => new SelectListItem
                            {
                                Value = c.CategoryID.ToString(),
                                Text = c.Name
                            }).ToList();
                        }
                    }
                    catch
                    {
                        // Fallback: parse như array trực tiếp
                        try
                        {
                            var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(responseContent, options) ?? new List<CategoryViewModel>();
                            return categories
                                .Where(c => c.IsActive)
                                .Select(c => new SelectListItem
                                {
                                    Value = c.CategoryID.ToString(),
                                    Text = c.Name
                                }).ToList();
                        }
                        catch (Exception parseEx)
                        {
                            _logger.LogError(parseEx, "Error parsing categories response: {Content}", responseContent);
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"GetCategoryOptionsAsync failed with status: {response.StatusCode}, Content: {errorContent}");
                }

                return new List<SelectListItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCategoryOptionsAsync");
                return new List<SelectListItem>();
            }
        }
    }
}