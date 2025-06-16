using BookManagementApi.Models;
using BookManagementApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookManagementApi.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context) { _context = context; }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books.Where(b => b.IsActive).ToListAsync();
        }
        public async Task<Book> GetByIdAsync(int id)
        {
            return await _context.Books.FindAsync(id);
        }
        public async Task<bool> UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> HideAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            book.IsActive = false;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Book> AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> UnhideAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            book.IsActive = true;
            return await _context.SaveChangesAsync() > 0;
        }


    }
}
