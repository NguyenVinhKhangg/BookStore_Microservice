using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BookManagementApi.DTOs;
using BookManagementApi.Services;

namespace BookManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersDetailController : ControllerBase
    {
        private readonly OrdersService _ordersService;

        public OrdersDetailController(OrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpGet("GetOrderDetail")]
        public async Task<IActionResult> GetOrderDetail(int orderDetailId)
        {
            try
            {
                var orderDetail = await _ordersService.GetOrderDetailByIdAsync(orderDetailId);
                return Ok(orderDetail);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }
}