using CartManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Services
{
    public interface ICartService
    {
        // Cart operations
        Task<IEnumerable<CartReadDto>> GetAllAsync();
        Task<CartReadDto> GetByIdAsync(int cartId);
        Task<CartReadDto> GetByUserIdAsync(int userId);
        Task<CartReadDto> CreateAsync(CartCreateDto cartDto);
        Task UpdateAsync(int cartId, CartUpdateDto cartDto);
        Task DeleteAsync(int cartId);
        Task<CartSummaryDto> GetCartSummaryAsync(int userId);

        // CartItem operations
        Task<CartItemReadDto> AddItemToCartAsync(CartItemCreateDto cartItemDto);
        Task<CartItemReadDto> UpdateCartItemAsync(int cartItemId, CartItemUpdateDto cartItemDto);
        Task DeleteCartItemAsync(int cartItemId);
        Task<IEnumerable<CartItemReadDto>> GetCartItemsByUserIdAsync(int userId);
        Task<CartItemReadDto> GetCartItemAsync(int userId, int bookId);

        // Cart management
        Task ClearCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task<bool> IsBookInCartAsync(int userId, int bookId);
    }
}