using BookClient.Models.Cart;

namespace BookClient.Services.CartServices
{
    public interface ICartService
    {
        Task<CartListResponseModel> GetUserCartAsync();
        Task<CartActionResponseModel> AddToCartAsync(AddToCartViewModel model);
        Task<CartActionResponseModel> UpdateCartItemAsync(UpdateCartItemViewModel model);
        Task<CartActionResponseModel> RemoveFromCartAsync(int cartId);
        Task<CartActionResponseModel> ClearCartAsync();
        Task<int> GetCartItemCountAsync();
    }
}