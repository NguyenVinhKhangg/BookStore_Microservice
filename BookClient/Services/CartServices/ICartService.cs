using BookClient.Models.Cart;

namespace BookClient.Services.CartServices
{
    public interface ICartService
    {
        Task<CartViewModel> GetCartByUserIdAsync(int userId);
        Task<CartItemViewModel> AddToCartAsync(int userId, AddToCartRequest request);
        Task<CartItemViewModel> UpdateCartItemAsync(UpdateCartItemRequest request);
        Task<bool> RemoveFromCartAsync(int cartItemId);
        Task<bool> ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<bool> IsBookInCartAsync(int userId, int bookId);
    }
}