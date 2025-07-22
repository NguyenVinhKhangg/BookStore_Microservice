using AutoMapper;
using CartItemManagementApi.DTOs;
using CartItemManagementApi.Models;
using CartItemManagementApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartItemManagementApi.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repository;
        private readonly IMapper _mapper;

        public CartItemService(ICartItemRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartItemReadDto>> GetAllAsync()
        {
            var items = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CartItemReadDto>>(items);
        }

        public async Task<CartItemReadDto> GetByIdAsync(int cartItemId)
        {
            var item = await _repository.GetByIdAsync(cartItemId);
            if (item == null)
                throw new KeyNotFoundException("CartItem not found");
            return _mapper.Map<CartItemReadDto>(item);
        }

        public async Task<CartItemReadDto> AddAsync(CartItemCreateDto dto)
        {
            // Nếu đã có CartID + BookID thì tăng số lượng, ngược lại thêm mới
            var existing = await _repository.GetByCartIdAndBookIdAsync(dto.CartID, dto.BookID);
            if (existing != null)
            {
                existing.Quantity += dto.Quantity;
                existing.AddedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(existing);
                return _mapper.Map<CartItemReadDto>(existing);
            }
            var entity = _mapper.Map<CartItems>(dto);
            entity.AddedAt = DateTime.UtcNow;
            entity.IsActive = true;
            await _repository.AddAsync(entity);
            return _mapper.Map<CartItemReadDto>(entity);
        }

        public async Task UpdateAsync(int cartItemId, CartItemUpdateDto dto)
        {
            if (cartItemId != dto.CartItemID)
                throw new ArgumentException("CartItemID mismatch");

            var item = await _repository.GetByIdAsync(cartItemId);
            if (item == null)
                throw new KeyNotFoundException("CartItem not found");

            item.Quantity = dto.Quantity;
            item.AddedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(item);
        }

        public async Task DeleteAsync(int cartItemId)
        {
            var item = await _repository.GetByIdAsync(cartItemId);
            if (item == null)
                throw new KeyNotFoundException("CartItem not found");
            await _repository.DeleteAsync(cartItemId);
        }
    }
}
