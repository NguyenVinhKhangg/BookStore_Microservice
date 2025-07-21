using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OrdersManagementApi.Data
{
    public class OrderDbContextFactory : IDesignTimeDbContextFactory<OrderDbContext>
    {
        public OrderDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderDbContext>();
            var connectionString = "server=DESKTOP-KE8KP90;database=BookStore_User;uid=sa;pwd=123;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connectionString);

            return new OrderDbContext(optionsBuilder.Options);
        }
    }
}