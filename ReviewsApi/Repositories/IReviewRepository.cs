using ReviewsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewsApi.Repositories
{
    public interface IReviewRepository
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> GetByIdAsync(int id);
        Task<Review> GetByBookIdAndUserIdAsync(int bookId, int userId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int id);
    }
}