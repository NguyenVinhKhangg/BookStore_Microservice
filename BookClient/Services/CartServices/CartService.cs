using BookClient.Models.Cart;
using BookClient.Services.CartServices;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BookClient.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _cartApiUrl;
        private const string TokenKey = "auth_token";
        private const string UserIdKey = "user_id";

        public CartService(
            HttpClient httpClient,
            ILogger<CartService> logger,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;

            var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7000";
            _cartApiUrl = $"{gatewayBaseUrl}/gateway/carts";

            _logger.LogInformation($"🛒 CartService initialized with Gateway: {gatewayBaseUrl}");
        }

        private async Task<string?> GetCurrentUserTokenAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return session?.GetString(TokenKey);
        }

        private async Task<int?> GetCurrentUserIdAsync()
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            var userIdStr = session?.GetString(UserIdKey);
            return int.TryParse(userIdStr, out var userId) ? userId : null;
        }

        private async Task SetAuthorizationHeaderAsync()
        {
            var token = await GetCurrentUserTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<CartListResponseModel> GetUserCartAsync()
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                if (!userId.HasValue)
                {
                    return new CartListResponseModel
                    {
                        Success = false,
                        Message = "User not logged in"
                    };
                }

                await SetAuthorizationHeaderAsync();

                _logger.LogInformation($"🛒 Getting cart for user: {userId.Value}");

                // Get user's cart items via OData filter
                var url = $"{_cartApiUrl}?$filter=userID eq {userId.Value}";
                var response = await _httpClient.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"📡 Cart API Response Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    // Parse cart items from API
                    var cartItems = JsonSerializer.Deserialize<List<CartApiResponse>>(responseContent, jsonOptions) ?? new List<CartApiResponse>();

                    // Enrich with book information
                    var cartViewModels = new List<CartViewModel>();
                    foreach (var item in cartItems)
                    {
                        var bookInfo = await GetBookInfoAsync(item.BookID);
                        if (bookInfo != null)
                        {
                            cartViewModels.Add(new CartViewModel
                            {
                                CartID = item.CartID,
                                UserID = item.UserID,
                                BookID = item.BookID,
                                BookTitle = bookInfo.Title,
                                AuthorName = bookInfo.AuthorName,
                                ImageUrl = bookInfo.ImageUrl,
                                CategoryName = bookInfo.CategoryName,
                                UnitPrice = bookInfo.Price,
                                Discount = bookInfo.Discount,
                                Quantity = item.Quantity,
                                Stock = bookInfo.Stock,
                                CreatedAt = item.CreatedAt
                            });
                        }
                    }

                    _logger.LogInformation($"✅ Retrieved {cartViewModels.Count} cart items successfully");

                    return new CartListResponseModel
                    {
                        Success = true,
                        Data = cartViewModels,
                        TotalCount = cartViewModels.Count
                    };
                }
                else
                {
                    _logger.LogError($"❌ Cart API failed: {response.StatusCode} - {responseContent}");
                    return new CartListResponseModel
                    {
                        Success = false,
                        Message = $"Failed to get cart: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception getting user cart");
                return new CartListResponseModel
                {
                    Success = false,
                    Message = "An error occurred while retrieving cart."
                };
            }
        }

        public async Task<CartActionResponseModel> AddToCartAsync(AddToCartViewModel model)
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                if (!userId.HasValue)
                {
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = "User not logged in"
                    };
                }

                await SetAuthorizationHeaderAsync();

                _logger.LogInformation($"🛒 Adding book {model.BookID} to cart for user: {userId.Value}");

                var cartData = new
                {
                    userID = userId.Value,
                    bookID = model.BookID,
                    quantity = model.Quantity
                };

                var jsonContent = JsonSerializer.Serialize(cartData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_cartApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var totalItems = await GetCartItemCountAsync();

                    _logger.LogInformation($"✅ Successfully added book {model.BookID} to cart");

                    return new CartActionResponseModel
                    {
                        Success = true,
                        Message = "Item added to cart successfully",
                        TotalItems = totalItems
                    };
                }
                else
                {
                    _logger.LogError($"❌ Add to cart failed: {response.StatusCode} - {responseContent}");
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to add item to cart: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception adding to cart");
                return new CartActionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while adding item to cart."
                };
            }
        }

        public async Task<CartActionResponseModel> UpdateCartItemAsync(UpdateCartItemViewModel model)
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                if (!userId.HasValue)
                {
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = "User not logged in"
                    };
                }

                await SetAuthorizationHeaderAsync();

                _logger.LogInformation($"🛒 Updating cart item {model.CartID} quantity to {model.Quantity}");

                var updateData = new
                {
                    cartID = model.CartID,
                    quantity = model.Quantity
                };

                var jsonContent = JsonSerializer.Serialize(updateData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"{_cartApiUrl}/{model.CartID}", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✅ Successfully updated cart item {model.CartID}");

                    return new CartActionResponseModel
                    {
                        Success = true,
                        Message = "Cart item updated successfully"
                    };
                }
                else
                {
                    _logger.LogError($"❌ Update cart failed: {response.StatusCode} - {responseContent}");
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to update cart item: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception updating cart item");
                return new CartActionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while updating cart item."
                };
            }
        }

        public async Task<CartActionResponseModel> RemoveFromCartAsync(int cartId)
        {
            try
            {
                await SetAuthorizationHeaderAsync();

                _logger.LogInformation($"🛒 Removing cart item {cartId}");

                var response = await _httpClient.DeleteAsync($"{_cartApiUrl}/{cartId}");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"✅ Successfully removed cart item {cartId}");

                    return new CartActionResponseModel
                    {
                        Success = true,
                        Message = "Item removed from cart successfully"
                    };
                }
                else
                {
                    _logger.LogError($"❌ Remove from cart failed: {response.StatusCode} - {responseContent}");
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = $"Failed to remove item from cart: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception removing from cart");
                return new CartActionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while removing item from cart."
                };
            }
        }

        public async Task<CartActionResponseModel> ClearCartAsync()
        {
            try
            {
                var userId = await GetCurrentUserIdAsync();
                if (!userId.HasValue)
                {
                    return new CartActionResponseModel
                    {
                        Success = false,
                        Message = "User not logged in"
                    };
                }

                // Get all cart items and delete them
                var cartItems = await GetUserCartAsync();
                if (cartItems.Success && cartItems.Data != null)
                {
                    foreach (var item in cartItems.Data)
                    {
                        await RemoveFromCartAsync(item.CartID);
                    }
                }

                return new CartActionResponseModel
                {
                    Success = true,
                    Message = "Cart cleared successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception clearing cart");
                return new CartActionResponseModel
                {
                    Success = false,
                    Message = "An error occurred while clearing cart."
                };
            }
        }

        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                var cart = await GetUserCartAsync();
                return cart.Success && cart.Data != null ? cart.Data.Sum(i => i.Quantity) : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Exception getting cart item count");
                return 0;
            }
        }

        private async Task<BookInfo?> GetBookInfoAsync(int bookId)
        {
            try
            {
                var gatewayBaseUrl = _configuration["ApiGateway:BaseUrl"] ?? "https://localhost:7000";
                var bookApiUrl = $"{gatewayBaseUrl}/gateway/books/{bookId}/detailBook";

                var response = await _httpClient.GetAsync(bookApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var jsonOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<BookInfo>(responseContent, jsonOptions);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"💥 Exception getting book info for {bookId}");
                return null;
            }
        }

        // Helper classes
        private class CartApiResponse
        {
            public int CartID { get; set; }
            public int UserID { get; set; }
            public int BookID { get; set; }
            public int Quantity { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        private class BookInfo
        {
            public string Title { get; set; } = "";
            public string AuthorName { get; set; } = "";
            public string? ImageUrl { get; set; }
            public string CategoryName { get; set; } = "";
            public decimal Price { get; set; }
            public decimal Discount { get; set; }
            public int Stock { get; set; }
        }
    }
}