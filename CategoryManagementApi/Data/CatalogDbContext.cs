using Microsoft.EntityFrameworkCore;
using CategoryManagementApi.Models;

namespace CategoryManagementApi.Data
{
    public class CatalogDbContext : DbContext
    {
        public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

        public DbSet<Category> Categories { get; set; }
    }
}