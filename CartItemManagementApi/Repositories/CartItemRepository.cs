using CartItemManagementApi.Data;
using CartItemManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartItemManagementApi.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly CartItemDbContext _context;

        public CartItemRepository(CartItemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItems>> GetAllAsync()
        {
            return await _context.CartItems.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<CartItems?> GetByIdAsync(int cartItemId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(x => x.CartItemID == cartItemId && x.IsActive);
        }

        public async Task<CartItems?> GetByCartIdAndBookIdAsync(int cartId, int bookId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(x => x.CartID == cartId && x.BookID == bookId && x.IsActive);
        }

        public async Task AddAsync(CartItems cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CartItems cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null && cartItem.IsActive)
            {
                cartItem.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }
    }
}
