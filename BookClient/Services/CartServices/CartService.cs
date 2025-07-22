using BookClient.Models.Cart;
using Newtonsoft.Json;
using System.Text;

namespace BookClient.Services.CartServices
{
    public class CartService : ICartService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CartService> _logger;

        public CartService(IHttpClientFactory httpClientFactory, ILogger<CartService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("CartAPI");
            _logger = logger;
        }

        public async Task<CartViewModel> GetCartByUserIdAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Carts/user/{userId}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CartViewModel>(content) ?? new CartViewModel { UserID = userId };
                }

                _logger.LogWarning($"Failed to get cart for user {userId}: {response.StatusCode}");
                return new CartViewModel { UserID = userId };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart for user {userId}");
                return new CartViewModel { UserID = userId };
            }
        }

        public async Task<CartItemViewModel> AddToCartAsync(int userId, AddToCartRequest request)
        {
            try
            {
                // ✅ FIX: Đầu tiên get hoặc tạo cart cho user
                var cart = await GetCartByUserIdAsync(userId);

                if (cart.CartID == 0)
                {
                    // Tạo cart mới nếu chưa có
                    cart = await CreateCartForUserAsync(userId);
                }

                // ✅ FIX: Tạo correct DTO structure for CartAPI
                var cartItemDto = new
                {
                    CartID = cart.CartID,  // ✅ Sử dụng CartID không phải UserId
                    BookID = request.BookID,
                    Quantity = request.Quantity,
                    Price = request.Price
                };

                var json = JsonConvert.SerializeObject(cartItemDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation($"Adding item to cart. CartID: {cart.CartID}, BookID: {request.BookID}, Quantity: {request.Quantity}, Price: {request.Price}");

                var response = await _httpClient.PostAsync("api/Carts/items", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CartItemViewModel>(responseContent) ?? new CartItemViewModel();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to add item to cart: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to add item to cart: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding item to cart for user {userId}");
                throw;
            }
        }

        private async Task<CartViewModel> CreateCartForUserAsync(int userId)
        {
            try
            {
                var createCartDto = new { UserID = userId };
                var json = JsonConvert.SerializeObject(createCartDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Carts", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CartViewModel>(responseContent) ?? new CartViewModel { UserID = userId };
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to create cart: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to create cart: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating cart for user {userId}");
                throw;
            }
        }

        public async Task<CartItemViewModel> UpdateCartItemAsync(UpdateCartItemRequest request)
        {
            try
            {
                var updateDto = new
                {
                    CartItemID = request.CartItemID,
                    Quantity = request.Quantity,
                    Price = request.Price // ✅ Include price if provided
                };

                var json = JsonConvert.SerializeObject(updateDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"api/Carts/items/{request.CartItemID}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<CartItemViewModel>(responseContent) ?? new CartItemViewModel();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError($"Failed to update cart item {request.CartItemID}: {response.StatusCode} - {errorContent}");
                throw new Exception($"Failed to update cart item: {response.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating cart item {request.CartItemID}");
                throw;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartItemId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Carts/items/{cartItemId}");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing cart item {cartItemId}");
                return false;
            }
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Carts/user/{userId}/clear");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error clearing cart for user {userId}");
                return false;
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Carts/user/{userId}/count");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<int>(content);
                }
                return 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cart count for user {userId}");
                return 0;
            }
        }

        public async Task<bool> IsBookInCartAsync(int userId, int bookId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Carts/user/{userId}/book/{bookId}/exists");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<bool>(content);
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if book {bookId} is in cart for user {userId}");
                return false;
            }
        }
    }
}