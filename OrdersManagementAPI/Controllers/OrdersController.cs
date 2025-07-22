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
        public async Task<IActionResult> GetOrders([FromQuery] string searchTerm = "", [FromQuery] string statusFilter = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var (orders, totalCount) = await _service.GetOrdersAsync(searchTerm, statusFilter, page, pageSize);
            return Ok(new { Orders = orders, TotalCount = totalCount });
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

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            try
            {
                await _service.ConfirmAsync(id);
                return Ok(new { message = "Order confirmed successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Order not found");
            }
        }
    }
}