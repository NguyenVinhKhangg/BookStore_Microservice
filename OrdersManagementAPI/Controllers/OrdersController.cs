using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BookManagementApi.DTOs;
using BookManagementApi.Services;

namespace BookManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet("GetOrderById")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _ordersService.GetOrderByIdAsync(orderId);
                return Ok(order);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrdersDTO dto)
        {
            if (dto == null || dto.TotalAmount < 0 || !new[] { "Pending", "Completed" }.Contains(dto.OrderStatus))
                return BadRequest("Invalid data.");

            var createdOrder = await _ordersService.CreateOrderAsync(dto);
            return CreatedAtAction(nameof(GetOrderById), new { orderId = createdOrder.OrderId }, createdOrder);
        }

        [HttpPost("UpdateStatusOrder")]
        public async Task<IActionResult> UpdateStatusOrder([FromBody] UpdateStatusOrderDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Status) || !new[] { "Pending", "Completed" }.Contains(dto.Status))
                return BadRequest("Invalid status value.");

            var result = await _ordersService.UpdateStatusOrderAsync(dto.OrderId, dto.Status);
            return result ? Ok($"Order {dto.OrderId} status updated to {dto.Status}") : NotFound();
        }

        [HttpPut("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] OrdersDTO dto)
        {
            if (dto == null || dto.TotalAmount < 0 || !new[] { "Pending", "Completed" }.Contains(dto.OrderStatus))
                return BadRequest("Invalid total amount or status.");

            var result = await _ordersService.UpdateOrderAsync(orderId, dto);
            return result ? Ok($"Order {orderId} updated") : NotFound();
        }

        [HttpGet("GetOrderByUserId")]
        public async Task<IActionResult> GetOrderByUserId(int userId)
        {
            try
            {
                var orders = await _ordersService.GetOrdersByUserIdAsync(userId);
                return Ok(orders);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost("GetOrderCancel")]
        public async Task<IActionResult> GetOrderCancel(int orderId)
        {
            var result = await _ordersService.CancelOrderAsync(orderId);
            return result ? Ok($"Order {orderId} cancelled") : NotFound();
        }
    }
}