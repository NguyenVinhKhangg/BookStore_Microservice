using CartManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Services
{
    public interface ICartService
    {
        Task<IEnumerable<CartReadDto>> GetAllAsync();
        Task<CartReadDto> GetByIdAsync(int cartId);
        Task<CartReadDto> CreateAsync(CartCreateDto cartDto);
        Task UpdateAsync(int cartId, CartUpdateDto cartDto);
        Task DeleteAsync(int cartId);
    }
}
