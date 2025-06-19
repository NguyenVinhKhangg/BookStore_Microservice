using BookImgManagementApi.Models;

namespace BookImgManagementApi.Repositories
{
    public interface IBooksImgRepository
    {
        Task<IEnumerable<BooksImg>> GetAllAsync();
        Task<BooksImg?> GetByIdAsync(int imageId);
        Task AddAsync(BooksImg img);
        Task UpdateAsync(BooksImg img);
        Task DeleteAsync(int imageId);
    }
}
