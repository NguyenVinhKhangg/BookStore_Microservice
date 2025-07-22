using BookImgManagementApi.DTOs;

namespace BookImgManagementApi.Services
{
    public interface IBooksImgService
    {
        Task<IEnumerable<BooksImgReadDto>> GetAllAsync();
        Task<BooksImgReadDto> GetByIdAsync(int imageId);
        Task<BooksImgReadDto> AddAsync(BooksImgCreateDto dto);
        Task UpdateAsync(int imageId, BooksImgUpdateDto dto);
        Task DeleteAsync(int imageId);
    }
}
