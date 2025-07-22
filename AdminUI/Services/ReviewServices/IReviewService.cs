using Microsoft.AspNetCore.Mvc.Rendering;

namespace AdminUI.Services.ReviewServices
{
    public interface IReviewService
    {
        Task<(List<ReviewReadDto> Reviews, int TotalCount)> GetAllAsync(string? searchTerm, bool? statusFilter, int? bookFilter, int pageNumber, int pageSize);
        Task<ReviewReadDto> GetByIdAsync(int id);
        Task<ReviewReadDto> CreateAsync(ReviewCreateDto reviewDto);
        Task UpdateAsync(int id, ReviewUpdateDto reviewDto);
        Task DeleteAsync(int id);
        Task<List<SelectListItem>> GetBookOptionsAsync();
        Task<List<SelectListItem>> GetUserOptionsAsync();
    }
}
