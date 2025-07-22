using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Services;
using System.Security.Claims;

namespace OrdersManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService service, ILogger<OrdersController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ✅ Get all orders (basic)
        [HttpGet]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetOrders()
        {
            try
            {
                var orders = await _service.GetAllAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all orders");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Get orders with pagination and filters (legacy compatibility)
        [HttpGet("search")]
        public async Task<IActionResult> GetOrdersWithFilters(
            [FromQuery] string searchTerm = "",
            [FromQuery] string statusFilter = "",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (orders, totalCount) = await _service.GetOrdersAsync(searchTerm, statusFilter, page, pageSize);

                return Ok(new
                {
                    Orders = orders,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders with filters");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Admin get orders với advanced filters
        [HttpGet("admin")]
        public async Task<IActionResult> GetOrdersForAdmin(
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? statusFilter = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (orders, totalCount) = await _service.GetOrdersForAdminAsync(
                    searchTerm, statusFilter, fromDate, toDate, page, pageSize);

                return Ok(new
                {
                    Orders = orders,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders for admin");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Get orders by user
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUser(
            int userId,
            [FromQuery] string? statusFilter = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (orders, totalCount) = await _service.GetOrdersByUserAsync(userId, statusFilter, page, pageSize);

                return Ok(new
                {
                    Orders = orders,
                    TotalCount = totalCount,
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for user {userId}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Get single order by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderReadDto>> GetOrder(int id)
        {
            try
            {
                var order = await _service.GetByIdAsync(id);

                if (order == null)
                    return NotFound(new { message = $"Order {id} not found" });

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order {id}");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Create new order
        [HttpPost]
        public async Task<ActionResult<OrderReadDto>> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _service.CreateOrderAsync(orderDto);

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Checkout endpoint (main entry point from cart)
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderReadDto>> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var order = await _service.CheckoutFromCartAsync(checkoutDto);

                return CreatedAtAction(nameof(GetOrder), new { id = order.OrderID }, order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during checkout");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Update order
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto orderDto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var success = await _service.UpdateOrderAsync(id, orderDto);

        //        if (!success)
        //            return NotFound(new { message = $"Order {id} not found" });

        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, $"Error updating order {id}");
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        // ✅ Admin update order status
        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderReadDto>> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto statusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var adminUserId = GetCurrentUserId();
                var order = await _service.UpdateOrderStatusAsync(id, statusDto, adminUserId);

                return Ok(order);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Order {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order {id} status");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Confirm order (legacy compatibility)
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
                return NotFound(new { message = $"Order {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error confirming order {id}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Delete order
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var success = await _service.DeleteOrderAsync(id);

                if (!success)
                    return NotFound(new { message = $"Order {id} not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order {id}");
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Statistics endpoints
        [HttpGet("statistics/count")]
        public async Task<ActionResult<int>> GetOrderCount([FromQuery] string? statusFilter = null)
        {
            try
            {
                var count = await _service.GetOrderCountAsync(statusFilter);
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order count");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("statistics/revenue")]
        public async Task<ActionResult<decimal>> GetTotalRevenue(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var revenue = await _service.GetTotalRevenueAsync(fromDate, toDate);
                return Ok(revenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total revenue");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("statistics/status")]
        public async Task<ActionResult<IDictionary<string, int>>> GetOrderStatusStats()
        {
            try
            {
                var stats = await _service.GetOrderStatusStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order status stats");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        // ✅ Helper method
        private int GetCurrentUserId()
        {
            // TODO: Extract from JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
                return userId;

            return 1; // Default for now
        }
    }
}