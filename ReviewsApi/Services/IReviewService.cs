using ReviewsApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReviewsApi.Services
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewReadDto>> GetAllAsync();
        Task<ReviewReadDto> GetByIdAsync(int id);
        Task<ReviewReadDto> CreateAsync(ReviewCreateDto reviewDto);
        Task UpdateAsync(int id, ReviewUpdateDto reviewDto);
        Task DeleteAsync(int id);
    }
}