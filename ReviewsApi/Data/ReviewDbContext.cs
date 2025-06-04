using Microsoft.EntityFrameworkCore;
using ReviewsApi.Models;

namespace ReviewsApi.Data
{
    public class ReviewDbContext : DbContext
    {
        public ReviewDbContext(DbContextOptions<ReviewDbContext> options) : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }
    }
}
