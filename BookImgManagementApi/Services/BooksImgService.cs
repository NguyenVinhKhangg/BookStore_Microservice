using AutoMapper;
using BookImgManagementApi.DTOs;
using BookImgManagementApi.Models;
using BookImgManagementApi.Repositories;

namespace BookImgManagementApi.Services
{
    public class BooksImgService : IBooksImgService
    {
        private readonly IBooksImgRepository _repo;
        private readonly IMapper _mapper;

        public BooksImgService(IBooksImgRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BooksImgReadDto>> GetAllAsync()
        {
            var imgs = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<BooksImgReadDto>>(imgs);
        }

        public async Task<BooksImgReadDto> GetByIdAsync(int imageId)
        {
            var img = await _repo.GetByIdAsync(imageId);
            if (img == null) throw new KeyNotFoundException("Image not found");
            return _mapper.Map<BooksImgReadDto>(img);
        }

        public async Task<BooksImgReadDto> AddAsync(BooksImgCreateDto dto)
        {
            var entity = _mapper.Map<BooksImg>(dto);
            entity.UploadedAt = DateTime.UtcNow;
            await _repo.AddAsync(entity);
            return _mapper.Map<BooksImgReadDto>(entity);
        }

        public async Task UpdateAsync(int imageId, BooksImgUpdateDto dto)
        {
            if (imageId != dto.ImageID)
                throw new ArgumentException("ImageID mismatch");
            var img = await _repo.GetByIdAsync(imageId);
            if (img == null) throw new KeyNotFoundException("Image not found");
            _mapper.Map(dto, img);
            await _repo.UpdateAsync(img);
        }

        public async Task DeleteAsync(int imageId)
        {
            var img = await _repo.GetByIdAsync(imageId);
            if (img == null) throw new KeyNotFoundException("Image not found");
            await _repo.DeleteAsync(imageId);
        }
    }
}
