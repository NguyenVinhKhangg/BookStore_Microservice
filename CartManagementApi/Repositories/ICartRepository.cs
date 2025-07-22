using CartManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Repository
{
    public interface ICartRepository
    {
        Task<IEnumerable<Cart>> GetAllAsync();
        Task<Cart> GetByIdAsync(int cartId);
        Task<Cart> GetByUserIdAsync(int userId);
        Task<Cart> AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task DeleteAsync(int cartId);
    }
}