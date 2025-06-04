using ReviewsApi.Data;
using ReviewsApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewsApi.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ReviewDbContext _context;

        public ReviewRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _context.Reviews
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task<Review> GetByBookIdAndUserIdAsync(int bookId, int userId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(r => r.BookID == bookId && r.UserID == userId);
        }

        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Entry(review).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null) {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}