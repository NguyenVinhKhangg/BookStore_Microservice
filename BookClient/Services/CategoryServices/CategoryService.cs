using BookClient.Models;
using BookClient.Services.CategoryServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookClient.Services.CategoryServices
{
    public class CategoryService : ICategoryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CategoryService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _categoryODataUrl;
        private readonly string _categoryApiUrl;

        public CategoryService(HttpClient httpClient, ILogger<CategoryService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;

            var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7000";

            // ✅ SỬA: Sử dụng OData endpoint có sẵn
            _categoryODataUrl = $"{gatewayBaseUrl}/gateway/odata/categories";
            _categoryApiUrl = $"{gatewayBaseUrl}/gateway/categories";

            _logger.LogInformation($"🔧 CategoryService initialized with Gateway OData: {_categoryODataUrl}");
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            try
            {
                // ✅ SỬA: Sử dụng OData endpoint với filter chỉ lấy active categories
                var odataUrl = $"{_categoryODataUrl}?$filter=isActive eq true&$orderby=name";

                _logger.LogInformation($"📂 Getting all categories from OData: {odataUrl}");

                var response = await _httpClient.GetAsync(odataUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📡 Category OData Response Status: {response.StatusCode}");
                _logger.LogInformation($"📄 Response content: {responseContent.Substring(0, Math.Min(500, responseContent.Length))}...");

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // ✅ Parse OData response structure
                    List<Category> categories;

                    if (responseContent.Contains("\"value\""))
                    {
                        // OData format: { "value": [...] }
                        var odataResponse = JsonSerializer.Deserialize<ODataResponse<Category>>(responseContent, jsonOptions);
                        categories = odataResponse?.Value ?? new List<Category>();

                        _logger.LogInformation($"✅ Retrieved {categories.Count} categories from OData (with value wrapper)");
                    }
                    else
                    {
                        // Direct array format: [...]
                        categories = JsonSerializer.Deserialize<List<Category>>(responseContent, jsonOptions) ?? new List<Category>();

                        _logger.LogInformation($"✅ Retrieved {categories.Count} categories from OData (direct array)");
                    }

                    return categories;
                }
                else
                {
                    _logger.LogError($"❌ Category OData API failed: {response.StatusCode} - {responseContent}");
                    return new List<Category>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception getting categories from OData");
                return new List<Category>();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation($"📂 Getting category by ID: {id}");

                // ✅ Có thể dùng OData hoặc direct API cho single item
                // Sử dụng OData với filter
                var odataUrl = $"{_categoryODataUrl}?$filter=categoryID eq {id}";

                var response = await _httpClient.GetAsync(odataUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📡 Category OData Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    Category? category = null;

                    if (responseContent.Contains("\"value\""))
                    {
                        // OData format: { "value": [...] }
                        var odataResponse = JsonSerializer.Deserialize<ODataResponse<Category>>(responseContent, jsonOptions);
                        category = odataResponse?.Value?.FirstOrDefault();
                    }
                    else
                    {
                        // Direct array format: [...]
                        var categories = JsonSerializer.Deserialize<List<Category>>(responseContent, jsonOptions);
                        category = categories?.FirstOrDefault();
                    }

                    _logger.LogInformation($"✅ Retrieved category: {category?.Name ?? "null"}");
                    return category;
                }
                else
                {
                    _logger.LogError($"❌ Category OData API failed: {response.StatusCode} - {responseContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception getting category {id} from OData");
                return null;
            }
        }

        // ✅ THÊM: Method để search categories
        public async Task<List<Category>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllCategoriesAsync();
                }

                // ✅ Sử dụng OData với contains filter
                var odataUrl = $"{_categoryODataUrl}?$filter=isActive eq true and contains(tolower(name), '{searchTerm.ToLower()}')&$orderby=name";

                _logger.LogInformation($"🔍 Searching categories with term '{searchTerm}': {odataUrl}");

                var response = await _httpClient.GetAsync(odataUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    List<Category> categories;

                    if (responseContent.Contains("\"value\""))
                    {
                        var odataResponse = JsonSerializer.Deserialize<ODataResponse<Category>>(responseContent, jsonOptions);
                        categories = odataResponse?.Value ?? new List<Category>();
                    }
                    else
                    {
                        categories = JsonSerializer.Deserialize<List<Category>>(responseContent, jsonOptions) ?? new List<Category>();
                    }

                    _logger.LogInformation($"✅ Found {categories.Count} categories matching '{searchTerm}'");
                    return categories;
                }
                else
                {
                    _logger.LogError($"❌ Category search failed: {response.StatusCode} - {responseContent}");
                    return new List<Category>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception searching categories with term: {searchTerm}");
                return new List<Category>();
            }
        }

        // ✅ THÊM: Method để get categories với pagination
        public async Task<(List<Category> Categories, int TotalCount)> GetCategoriesWithPaginationAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                // ✅ Sử dụng OData với pagination và count
                var odataUrl = $"{_categoryODataUrl}?$filter=isActive eq true&$orderby=name&$skip={skip}&$top={pageSize}&$count=true";

                _logger.LogInformation($"📄 Getting categories with pagination: {odataUrl}");

                var response = await _httpClient.GetAsync(odataUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    if (responseContent.Contains("\"value\""))
                    {
                        var odataResponse = JsonSerializer.Deserialize<ODataResponseWithCount<Category>>(responseContent, jsonOptions);
                        var categories = odataResponse?.Value ?? new List<Category>();
                        var totalCount = odataResponse?.Count ?? categories.Count;

                        _logger.LogInformation($"✅ Retrieved {categories.Count} categories (page {page}, total: {totalCount})");
                        return (categories, totalCount);
                    }
                    else
                    {
                        var categories = JsonSerializer.Deserialize<List<Category>>(responseContent, jsonOptions) ?? new List<Category>();
                        _logger.LogInformation($"✅ Retrieved {categories.Count} categories (page {page})");
                        return (categories, categories.Count);
                    }
                }
                else
                {
                    _logger.LogError($"❌ Category pagination failed: {response.StatusCode} - {responseContent}");
                    return (new List<Category>(), 0);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception getting categories with pagination");
                return (new List<Category>(), 0);
            }
        }
    }

    // ✅ THÊM: OData response models
    public class ODataResponse<T>
    {
        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new List<T>();

        [JsonPropertyName("@odata.context")]
        public string? ODataContext { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string? ODataNextLink { get; set; }
    }

    public class ODataResponseWithCount<T> : ODataResponse<T>
    {
        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }
    }
}