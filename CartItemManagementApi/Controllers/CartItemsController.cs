using CartItemManagementApi.DTOs;
using CartItemManagementApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartItemManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemsController : ControllerBase
    {
        private readonly ICartItemService _service;

        public CartItemsController(ICartItemService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<CartItemReadDto>>> GetCartItems()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CartItemReadDto>> GetCartItem(int id)
        {
            try
            {
                var item = await _service.GetByIdAsync(id);
                return Ok(item);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("CartItem not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CartItemReadDto>> PostCartItem(CartItemCreateDto dto)
        {
            var item = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetCartItem), new { id = item.CartItemID }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCartItem(int id, CartItemUpdateDto dto)
        {
            if (id != dto.CartItemID)
                return BadRequest("CartItemID mismatch");
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("CartItem not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("CartItem not found");
            }
        }
    }
}
