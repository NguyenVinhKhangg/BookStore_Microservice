using Microsoft.EntityFrameworkCore;
using BookManagementApi.Models;
using System.Collections.Generic;

namespace BookManagementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Book> Books { get; set; }
        public DbSet<BooksImg> BooksImgs { get; set; }

    }
}
