using BookImgManagementApi.Data;
using BookImgManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookImgManagementApi.Repositories
{
    public class BooksImgRepository : IBooksImgRepository
    {
        private readonly BookImgDbContext _context;
        public BooksImgRepository(BookImgDbContext context) => _context = context;

        public async Task<IEnumerable<BooksImg>> GetAllAsync() =>
            await _context.BooksImgs.ToListAsync();

        public async Task<BooksImg?> GetByIdAsync(int imageId) =>
            await _context.BooksImgs.FindAsync(imageId);

        public async Task AddAsync(BooksImg img)
        {
            _context.BooksImgs.Add(img);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BooksImg img)
        {
            _context.BooksImgs.Update(img);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int imageId)
        {
            var img = await _context.BooksImgs.FindAsync(imageId);
            if (img != null) {
                _context.BooksImgs.Remove(img);
                await _context.SaveChangesAsync();
            }
        }
    }
}
