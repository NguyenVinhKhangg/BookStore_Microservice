using Microsoft.AspNetCore.Mvc;
using CartManagementApi.Services;
using CartManagementApi.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace CartManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartsController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CartReadDto>>> GetAll()
        {
            var carts = await _cartService.GetAllAsync();
            return Ok(carts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartReadDto>> GetById(int id)
        {
            var cart = await _cartService.GetByIdAsync(id);
            if (cart == null)
                return NotFound();
            return Ok(cart);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartReadDto>> GetByUserId(int userId)
        {
            var cart = await _cartService.GetByUserIdAsync(userId);
            return Ok(cart);
        }

        [HttpGet("user/{userId}/summary")]
        public async Task<ActionResult<CartSummaryDto>> GetCartSummary(int userId)
        {
            var summary = await _cartService.GetCartSummaryAsync(userId);
            return Ok(summary);
        }

        [HttpGet("user/{userId}/items")]
        public async Task<ActionResult<IEnumerable<CartItemReadDto>>> GetCartItems(int userId)
        {
            var items = await _cartService.GetCartItemsByUserIdAsync(userId);
            return Ok(items);
        }

        [HttpGet("user/{userId}/count")]
        public async Task<ActionResult<int>> GetCartItemCount(int userId)
        {
            var count = await _cartService.GetCartItemCountAsync(userId);
            return Ok(count);
        }

        [HttpPost]
        public async Task<ActionResult<CartReadDto>> Create(CartCreateDto cartDto)
        {
            var cart = await _cartService.CreateAsync(cartDto);
            return CreatedAtAction(nameof(GetById), new { id = cart.CartID }, cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartItemReadDto>> AddItemToCart(CartItemCreateDto cartItemDto)
        {
            try
            {
                var cartItem = await _cartService.AddItemToCartAsync(cartItemDto);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CartUpdateDto cartDto)
        {
            await _cartService.UpdateAsync(id, cartDto);
            return NoContent();
        }

        [HttpPut("items/{cartItemId}")]
        public async Task<ActionResult<CartItemReadDto>> UpdateCartItem(int cartItemId, CartItemUpdateDto cartItemDto)
        {
            try
            {
                var cartItem = await _cartService.UpdateCartItemAsync(cartItemId, cartItemDto);
                return Ok(cartItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _cartService.DeleteAsync(id);
            return NoContent();
        }

        [HttpDelete("items/{cartItemId}")]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            await _cartService.DeleteCartItemAsync(cartItemId);
            return NoContent();
        }

        [HttpDelete("user/{userId}/clear")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return NoContent();
        }

        [HttpGet("user/{userId}/book/{bookId}/exists")]
        public async Task<ActionResult<bool>> IsBookInCart(int userId, int bookId)
        {
            var exists = await _cartService.IsBookInCartAsync(userId, bookId);
            return Ok(exists);
        }
    }
}