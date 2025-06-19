using CartManagementApi.DTOs;
using CartManagementApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _service;

        public CartsController(ICartService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CartReadDto>>> GetCarts()
        {
            var carts = await _service.GetAllAsync();
            return Ok(carts);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartReadDto>> GetCart(int id)
        {
            try
            {
                var cart = await _service.GetByIdAsync(id);
                return Ok(cart);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Cart not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CartReadDto>> PostCart(CartCreateDto cartDto)
        {
            var cart = await _service.CreateAsync(cartDto);
            return CreatedAtAction(nameof(GetCart), new { id = cart.CartID }, cart);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int id, CartUpdateDto cartDto)
        {
            if (id != cartDto.CartID)
                return BadRequest("CartID mismatch");
            try
            {
                await _service.UpdateAsync(id, cartDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Cart not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Cart not found");
            }
        }
    }
}
