using CartManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CartManagementApi.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }

        public DbSet<Cart> Carts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.UserID, c.BookID })
                .IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    }
}