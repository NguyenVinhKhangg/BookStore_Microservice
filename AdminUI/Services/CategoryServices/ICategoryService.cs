using AdminUI.Models.Category;

namespace AdminUI.Services.CategoryServices
{
    public interface ICategoryService
    {
        Task<CategoriesListResponseModel> GetCategoriesAsync(CategorySearchFilterViewModel filter);
        Task<CategoryResponseModel> GetCategoryByIdAsync(int id);
        Task<CategoryResponseModel> CreateCategoryAsync(CreateCategoryViewModel model);
        Task<CategoryResponseModel> UpdateCategoryAsync(UpdateCategoryViewModel model);
        Task<CategoryResponseModel> DeleteCategoryAsync(int id);
        Task<CategoryResponseModel> ActivateCategoryAsync(int id);
        Task<CategoryResponseModel> DeactivateCategoryAsync(int id);
    }
}
