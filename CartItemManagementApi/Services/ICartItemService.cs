using CartItemManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartItemManagementApi.Services
{
    public interface ICartItemService
    {
        Task<IEnumerable<CartItemReadDto>> GetAllAsync();
        Task<CartItemReadDto> GetByIdAsync(int cartItemId);
        Task<CartItemReadDto> AddAsync(CartItemCreateDto dto);
        Task UpdateAsync(int cartItemId, CartItemUpdateDto dto);
        Task DeleteAsync(int cartItemId);
    }
}
