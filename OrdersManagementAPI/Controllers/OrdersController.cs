using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Services; // Thêm directive này

namespace OrdersManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrdersController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders()
        {
            var orders = await _service.GetAllAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDto>> GetOrder(int id)
        {
            try
            {
                var order = await _service.GetByIdAsync(id);
                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> PostOrder(OrderCreateDto dto)
        {
            var order = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, OrderUpdateDto dto)
        {
            if (id != dto.OrderID)
                return BadRequest("OrderID mismatch");
            try
            {
                await _service.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
        }
    }
}