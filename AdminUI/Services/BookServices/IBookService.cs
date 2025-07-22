using AdminUI.Models.Book;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminUI.Services.BookServices
{
    public interface IBookService
    {
        Task<(List<BookResponseModel> books, int totalCount)> GetAllAsync(
            string? searchTerm = null,
            int? categoryFilter = null,
            bool? isActiveFilter = null,
            string? sortBy = "Title",
            string? sortOrder = "asc",
            int page = 1,
            int pageSize = 10);

        Task<BookResponseModel> GetByIdAsync(int id);
        Task<BookResponseModel> CreateAsync(CreateBookViewModel model);
        Task<bool> UpdateAsync(int id, UpdateBookViewModel model);
        Task<bool> HideAsync(int id);
        Task<bool> UnhideAsync(int id);
        Task<List<SelectListItem>> GetCategoryOptionsAsync();
    }
}
