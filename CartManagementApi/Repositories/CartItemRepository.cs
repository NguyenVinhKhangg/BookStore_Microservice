using CartManagementApi.Data;
using CartManagementApi.Models;
using CartManagementApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly CartDbContext _context;

        public CartItemRepository(CartDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetAllAsync()
        {
            return await _context.CartItems.ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(int cartItemId)
        {
            return await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId)
        {
            return await _context.CartItems
                .Where(ci => ci.CartID == cartId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByCartIdAndBookIdAsync(int cartId, int bookId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.CartID == cartId && ci.BookID == bookId);
        }

        public async Task<CartItem> AddAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            return cartItem;
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.Entry(cartItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int cartItemId)
        {
            var cartItem = await _context.CartItems.FindAsync(cartItemId);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }
    }
}