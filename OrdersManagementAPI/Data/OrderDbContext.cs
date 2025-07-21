using Microsoft.EntityFrameworkCore;
using OrdersManagementApi.Models;

namespace OrdersManagementApi.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
    }
}