using CartManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CartManagementApi.Data
{
    public class CartDbContext : DbContext
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options)
        {
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cart configuration
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartID);
                entity.Property(e => e.UserID).IsRequired();
                entity.Property(e => e.CreatedAt).IsRequired();

                // Relationship with CartItems
                entity.HasMany(e => e.CartItems)
                    .WithOne(e => e.Cart)
                    .HasForeignKey(e => e.CartID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // CartItem configuration
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemID);
                entity.Property(e => e.CartID).IsRequired();
                entity.Property(e => e.BookID).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.Price).IsRequired().HasColumnType("decimal(18,2)");
                entity.Property(e => e.AddedAt).IsRequired();
                entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);

                // Unique constraint for Cart + Book combination
                entity.HasIndex(e => new { e.CartID, e.BookID }).IsUnique();
            });
        }
    }
}