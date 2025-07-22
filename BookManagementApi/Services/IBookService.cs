using BookManagementApi.DTOs;

namespace BookManagementApi.Services
{
    public interface IBookService
    {
        Task<BookDto> AddBookAsync(BookCreateDto dto);
        Task<IEnumerable<BookDto>> GetBooksAsync(); // Chỉ sách active
        Task<IEnumerable<BookDto>> GetAllBooksForAdminAsync(); // Tất cả sách cho admin
        Task<BookDto> GetBookDetailAsync(int id, bool isAdmin = false);
        Task<BookDto> GetBookDetailForAdminAsync(int id); // Chi tiết sách cho admin
        Task<bool> UpdateBookAsync(int bookId, BookUpdateDto dto);
        Task<bool> HideBookAsync(int id);
        Task<bool> UnhideBookAsync(int id);
        Task<bool> BookExistsAsync(int id);
        Task<bool> UpdateBookStockAsync(int bookId, int quantityChange);
    }
}