using BookImgManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookImgManagementApi.Data
{
    public class BookImgDbContext : DbContext
    {
        public BookImgDbContext(DbContextOptions<BookImgDbContext> options) : base(options) { }

        public DbSet<BooksImg> BooksImgs { get; set; }
    }
}
