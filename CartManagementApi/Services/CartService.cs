using AutoMapper;
using CartManagementApi.DTOs;
using CartManagementApi.Models;
using CartManagementApi.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartReadDto>> GetAllAsync()
        {
            var carts = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<CartReadDto>>(carts);
        }

        public async Task<CartReadDto> GetByIdAsync(int cartId)
        {
            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");
            return _mapper.Map<CartReadDto>(cart);
        }

        public async Task<CartReadDto> CreateAsync(CartCreateDto cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);
            cart.CreatedAt = DateTime.UtcNow;
            cart.UpdatedAt = DateTime.UtcNow;
            await _repository.AddAsync(cart);
            return _mapper.Map<CartReadDto>(cart);
        }

        public async Task UpdateAsync(int cartId, CartUpdateDto cartDto)
        {
            if (cartId != cartDto.CartID)
                throw new ArgumentException("CartID mismatch");

            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");

            cart.UserID = cartDto.UserID;
            cart.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(cart);
        }

        public async Task DeleteAsync(int cartId)
        {
            var cart = await _repository.GetByIdAsync(cartId);
            if (cart == null)
                throw new KeyNotFoundException("Cart not found");
            await _repository.DeleteAsync(cartId);
        }
    }
}
