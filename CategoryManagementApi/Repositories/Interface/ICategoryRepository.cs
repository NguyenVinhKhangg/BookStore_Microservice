using CategoryManagementApi.Models;

namespace CategoryManagementApi.Repositories.Interface
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> AddAsync(Category category);
        Task<Category> UpdateAsync(Category category);
        Task<bool> DeleteAsync(int id);

        // Thêm cho OData
        IQueryable<Category> GetCategoriesQueryable();
    }
}