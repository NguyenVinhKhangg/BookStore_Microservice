using AutoMapper;
using AutoMapper.QueryableExtensions;
using CategoryManagementApi.DTOs;
using CategoryManagementApi.Models;
using CategoryManagementApi.Repositories.Interface;
using CategoryManagementApi.Services.Interface;

namespace CategoryManagementApi.Services.Implement
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            return category == null ? null : _mapper.Map<CategoryDTO>(category);
        }

        public async Task<CategoryDTO> CreateAsync(CreateCategoryDTO dto)
        {
            var category = _mapper.Map<Category>(dto);
            category.IsActive = true;
            var created = await _repo.AddAsync(category);
            return _mapper.Map<CategoryDTO>(created);
        }

        public async Task<CategoryDTO> UpdateAsync(int id, UpdateCategoryDTO dto)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) throw new Exception("Category not found");
            _mapper.Map(dto, category);
            var updated = await _repo.UpdateAsync(category);
            return _mapper.Map<CategoryDTO>(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }

        // Thêm cho OData
        public IQueryable<CategoryDTO> GetCategoriesQueryable()
        {
            return _repo.GetCategoriesQueryable().ProjectTo<CategoryDTO>(_mapper.ConfigurationProvider);
        }

        public async Task<bool> ActivateAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return false;
            category.IsActive = true;
            await _repo.UpdateAsync(category);
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var category = await _repo.GetByIdAsync(id);
            if (category == null) return false;
            category.IsActive = false;
            await _repo.UpdateAsync(category);
            return true;
        }
    }
}