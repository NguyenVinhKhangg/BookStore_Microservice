using Microsoft.EntityFrameworkCore;
using BookManagementApi.Models;

namespace BookManagementApi.Data
{
    public class BookStoreContext : DbContext
    {
        public BookStoreContext(DbContextOptions<BookStoreContext> options)
            : base(options)
        {
        }

        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Cấu hình cho bảng Orders
            modelBuilder.Entity<Orders>(entity =>
            {
                entity.Property(e => e.OrderStatus)
                      .IsRequired(false); // Cho phép null
                entity.Property(e => e.Note)
                      .IsRequired(false); // Cho phép null
            });

            // Áp dụng check constraint bằng ToTable
            modelBuilder.Entity<Orders>().ToTable("Orders", t =>
                t.HasCheckConstraint("CK_OrderStatus", "\"OrderStatus\" IS NULL OR \"OrderStatus\" IN ('Pending', 'Completed')"));

            // Cấu hình cho bảng OrderDetails
            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasIndex(e => new { e.OrderId, e.BookId }).IsUnique();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Discount).HasColumnType("decimal(5,2)");
            });
        }
    }
}