using CartManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CartManagementApi.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<Cart> Carts { get; set; }
    }
}
