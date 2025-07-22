using Newtonsoft.Json;
using AdminUI.Models;
using AdminUI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AdminUI.Controllers
{
    [Route("Orders")]
    public class OrderManagementController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderManagementController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("Index")]
        public async Task<IActionResult> Index(string searchTerm = "", string statusFilter = "", int page = 1, int pageSize = 10)
        {
            var filter = new { SearchTerm = searchTerm, StatusFilter = statusFilter };
            var result = await _orderService.GetOrdersAsync(searchTerm, statusFilter, page, pageSize);

            ViewBag.TotalCount = result.TotalCount;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
            ViewBag.Filter = filter;

            return View(result.Orders);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost("Update")]
        public async Task<IActionResult> Update(Order order)
        {
            if (ModelState.IsValid)
            {
                var success = await _orderService.UpdateOrderAsync(order);
                if (success)
                {
                    return RedirectToAction("Details", new { id = order.OrderID });
                }
                ModelState.AddModelError("", "Cập nhật đơn hàng thất bại.");
            }
            return View("Edit", order);
        }
    }
}