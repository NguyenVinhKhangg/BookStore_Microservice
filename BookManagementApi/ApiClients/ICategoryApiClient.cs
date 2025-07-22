namespace BookManagementApi.ApiClients
{
    using BookManagementApi.DTOs;
    public interface ICategoryApiClient
    {
       
        Task<CategoryDTO?> GetCategoryByIdAsync(int id);
    }
}
