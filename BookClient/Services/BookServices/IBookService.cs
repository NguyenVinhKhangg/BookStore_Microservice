using BookClient.Models;
using BookClient.Models.Book;

namespace BookClient.Services.BookServices
{
    public interface IBookService
    {
        Task<List<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<List<Book>> GetBooksByCategoryAsync(int categoryId);
        Task<List<Book>> SearchBooksAsync(string searchTerm);

        // ✅ THÊM: Advanced filtering method
        Task<(List<Book> Books, int TotalCount)> GetBooksWithFilterAsync(SearchFilterViewModel filter);
    }
}