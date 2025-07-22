using BookManagementApi.Models;

namespace BookManagementApi.Repositories
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllAsync(); // Tất cả sách (bao gồm bị ẩn)
        Task<IEnumerable<Book>> GetAllActiveAsync(); // Chỉ sách active
        Task<Book> GetByIdAsync(int id);
        Task<Book> AddAsync(Book book);
        Task<bool> UpdateAsync(Book book);
        Task<bool> HideAsync(int id);
        Task<bool> UnhideAsync(int id);
    }
}