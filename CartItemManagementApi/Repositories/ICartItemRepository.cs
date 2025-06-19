using CartItemManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartItemManagementApi.Repositories
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItems>> GetAllAsync();
        Task<CartItems?> GetByIdAsync(int cartItemId);
        Task<CartItems?> GetByCartIdAndBookIdAsync(int cartId, int bookId);
        Task AddAsync(CartItems cartItem);
        Task UpdateAsync(CartItems cartItem);
        Task DeleteAsync(int cartItemId);
    }
}
