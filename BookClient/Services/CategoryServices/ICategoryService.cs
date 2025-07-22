using BookClient.Models;

namespace BookClient.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);

        // ✅ THÊM: Additional methods utilizing OData capabilities
        Task<List<Category>> SearchCategoriesAsync(string searchTerm);
        Task<(List<Category> Categories, int TotalCount)> GetCategoriesWithPaginationAsync(int page = 1, int pageSize = 10);
    }
}