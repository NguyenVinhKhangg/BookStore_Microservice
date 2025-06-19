using CartItemManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CartItemManagementApi.Data
{
    public class CartItemDbContext : DbContext
    {
        public CartItemDbContext(DbContextOptions<CartItemDbContext> options) : base(options) { }

        public DbSet<CartItems> CartItems { get; set; }
    }
}
