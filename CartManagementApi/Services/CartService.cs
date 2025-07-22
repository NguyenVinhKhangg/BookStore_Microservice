using AutoMapper;
using CartManagementApi.DTOs;
using CartManagementApi.Models;
using CartManagementApi.Repositories;
using CartManagementApi.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IMapper _mapper;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository, IMapper mapper)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CartReadDto>> GetAllAsync()
        {
            var carts = await _cartRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CartReadDto>>(carts);
        }

        public async Task<CartReadDto> GetByIdAsync(int cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return null;

            var cartDto = _mapper.Map<CartReadDto>(cart);

            // Load cart items
            var cartItems = await _cartItemRepository.GetByCartIdAsync(cartId);
            cartDto.CartItems = _mapper.Map<List<CartItemReadDto>>(cartItems);

            return cartDto;
        }

        public async Task<CartReadDto> GetByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                // Create new cart for user if doesn't exist
                var newCartDto = new CartCreateDto { UserID = userId };
                return await CreateAsync(newCartDto);
            }

            var cartDto = _mapper.Map<CartReadDto>(cart);

            // Load cart items
            var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.CartID);
            cartDto.CartItems = _mapper.Map<List<CartItemReadDto>>(cartItems);

            return cartDto;
        }

        public async Task<CartReadDto> CreateAsync(CartCreateDto cartDto)
        {
            var cart = _mapper.Map<Cart>(cartDto);
            cart.CreatedAt = DateTime.UtcNow;

            var addedCart = await _cartRepository.AddAsync(cart);
            var result = _mapper.Map<CartReadDto>(addedCart);
            result.CartItems = new List<CartItemReadDto>();

            return result;
        }

        public async Task UpdateAsync(int cartId, CartUpdateDto cartDto)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart != null)
            {
                _mapper.Map(cartDto, cart);
                await _cartRepository.UpdateAsync(cart);
            }
        }

        public async Task DeleteAsync(int cartId)
        {
            // Delete all cart items first
            var cartItems = await _cartItemRepository.GetByCartIdAsync(cartId);
            foreach (var item in cartItems)
            {
                await _cartItemRepository.DeleteAsync(item.CartItemID);
            }

            // Then delete cart
            await _cartRepository.DeleteAsync(cartId);
        }

        public async Task<CartSummaryDto> GetCartSummaryAsync(int userId)
        {
            var cart = await GetByUserIdAsync(userId);
            if (cart == null || !cart.CartItems.Any())
            {
                return new CartSummaryDto
                {
                    CartID = 0,
                    UserID = userId,
                    TotalItems = 0,
                    SubTotal = 0,
                    TotalDiscount = 0,
                    TotalAmount = 0,
                    LastModified = DateTime.UtcNow
                };
            }

            var totalItems = cart.CartItems.Sum(item => item.Quantity);
            var subTotal = cart.CartItems.Sum(item => item.TotalPrice);
            var totalDiscount = cart.CartItems.Sum(item =>
                item.BookDiscount.HasValue ? (item.TotalPrice - item.TotalDiscountedPrice) : 0);
            var totalAmount = subTotal - totalDiscount;

            return new CartSummaryDto
            {
                CartID = cart.CartID,
                UserID = cart.UserID,
                TotalItems = totalItems,
                SubTotal = subTotal,
                TotalDiscount = totalDiscount,
                TotalAmount = totalAmount,
                LastModified = cart.CartItems.Max(item => item.AddedAt)
            };
        }

        public async Task<CartItemReadDto> AddItemToCartAsync(CartItemCreateDto cartItemDto)
        {
            // ✅ FIX: Sử dụng CartID đúng cách - không phải UserId
            var cart = await _cartRepository.GetByIdAsync(cartItemDto.CartID);
            if (cart == null)
            {
                throw new ArgumentException("Cart not found");
            }

            // Check if item already exists in cart
            var existingItem = await _cartItemRepository.GetByCartIdAndBookIdAsync(cart.CartID, cartItemDto.BookID);
            if (existingItem != null)
            {
                // Update quantity
                existingItem.Quantity += cartItemDto.Quantity;
                await _cartItemRepository.UpdateAsync(existingItem);
                return _mapper.Map<CartItemReadDto>(existingItem);
            }

            // Add new item
            var cartItem = _mapper.Map<CartItem>(cartItemDto);
            cartItem.CartID = cart.CartID;
            cartItem.AddedAt = DateTime.UtcNow;

            var addedItem = await _cartItemRepository.AddAsync(cartItem);
            return _mapper.Map<CartItemReadDto>(addedItem);
        }

        public async Task<CartItemReadDto> UpdateCartItemAsync(int cartItemId, CartItemUpdateDto cartItemDto)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }

            _mapper.Map(cartItemDto, cartItem);
            await _cartItemRepository.UpdateAsync(cartItem);

            return _mapper.Map<CartItemReadDto>(cartItem);
        }

        public async Task DeleteCartItemAsync(int cartItemId)
        {
            await _cartItemRepository.DeleteAsync(cartItemId);
        }

        public async Task<IEnumerable<CartItemReadDto>> GetCartItemsByUserIdAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null)
            {
                return new List<CartItemReadDto>();
            }

            var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.CartID);
            return _mapper.Map<IEnumerable<CartItemReadDto>>(cartItems);
        }

        public async Task<CartItemReadDto> GetCartItemAsync(int userId, int bookId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart == null) return null;

            var cartItem = await _cartItemRepository.GetByCartIdAndBookIdAsync(cart.CartID, bookId);
            return _mapper.Map<CartItemReadDto>(cartItem);
        }

        public async Task ClearCartAsync(int userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            if (cart != null)
            {
                var cartItems = await _cartItemRepository.GetByCartIdAsync(cart.CartID);
                foreach (var item in cartItems)
                {
                    await _cartItemRepository.DeleteAsync(item.CartItemID);
                }
            }
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            var cartItems = await GetCartItemsByUserIdAsync(userId);
            return cartItems.Sum(item => item.Quantity);
        }

        public async Task<bool> IsBookInCartAsync(int userId, int bookId)
        {
            var cartItem = await GetCartItemAsync(userId, bookId);
            return cartItem != null;
        }
    }
}