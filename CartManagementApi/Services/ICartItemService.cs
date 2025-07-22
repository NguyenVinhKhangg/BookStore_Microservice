using CartManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Services
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemReadDto>> GetAllAsync();
        Task<CartItemReadDto> GetByIdAsync(int cartItemId);
        Task<IEnumerable<CartItemReadDto>> GetByCartIdAsync(int cartId);
        Task<IEnumerable<CartItemReadDto>> GetByUserIdAsync(int userId);
        Task<CartItemReadDto> CreateAsync(CartItemCreateDto cartItemDto);
        Task UpdateAsync(int cartItemId, CartItemUpdateDto cartItemDto);
        Task DeleteAsync(int cartItemId);
        Task<CartItemReadDto> GetByUserAndBookAsync(int userId, int bookId);
    }
}