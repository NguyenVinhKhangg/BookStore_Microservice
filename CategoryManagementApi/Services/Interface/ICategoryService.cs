using CategoryManagementApi.DTOs;

namespace CategoryManagementApi.Services.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);
        Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto);
        Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO dto);
        Task<bool> DeleteAsync(int id);

        // Thêm cho OData
        IQueryable<CategoryDTO> GetCategoriesQueryable();

        Task<bool> ActivateAsync(int id);
        Task<bool> DeactivateAsync(int id);
    }
}