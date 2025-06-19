using BookManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagementApi.Services
{
    public interface IBookService
    {
        Task<IEnumerable<BookDto>> GetBooksAsync();
        Task<BookDto> GetBookDetailAsync(int bookId);
        Task<BookDto> AddBookAsync(BookCreateDto dto);
        Task<bool> UpdateBookAsync(int bookId, BookUpdateDto dto);
        Task<bool> HideBookAsync(int bookId);
        Task<bool> BookExistsAsync(int id);
        Task<bool> UnhideBookAsync(int id);
        Task<bool> UpdateBookStockAsync(int bookId, int quantityChange);
    }
}
