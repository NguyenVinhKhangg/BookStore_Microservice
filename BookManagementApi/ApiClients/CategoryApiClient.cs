using BookManagementApi.DTOs;
using System.Net.Http.Json;

namespace BookManagementApi.ApiClients
{
    public class CategoryApiClient : ICategoryApiClient
    {
        private readonly HttpClient _http;
        public CategoryApiClient(HttpClient http) => _http = http;

        public async Task<CategoryDTO?> GetCategoryByIdAsync(int id)
        {
            var response = await _http.GetAsync($"api/categories/{id}");
            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CategoryDTO>();
        }
    }
}
