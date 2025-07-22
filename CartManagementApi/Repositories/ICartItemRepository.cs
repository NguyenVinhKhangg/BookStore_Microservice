using CartManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Repository
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetAllAsync();
        Task<CartItem> GetByIdAsync(int cartItemId);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task<CartItem> GetByCartIdAndBookIdAsync(int cartId, int bookId);
        Task<CartItem> AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task DeleteAsync(int cartItemId);
    }
}