using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using BookClient.Models;
using BookClient.Services;

namespace BookClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(int cartId)
        {
            var order = await _orderService.CreateOrderFromCartAsync(cartId);
            return View("~/Views/Orders/Order.cshtml", order);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.OrderID == 0)
            {
                return NotFound();
            }
            return View("~/Views/Orders/Order.cshtml", order);
        }

        public async Task<IActionResult> MyOrders(int page = 1, int pageSize = 5, string search = "", string status = "")
        {
            var orders = await _orderService.GetAllOrdersAsync(page, pageSize, search, status);
            ViewBag.TotalPages = (int)Math.Ceiling((double)100 / pageSize); // Giả định, cần lấy từ API
            ViewBag.CurrentPage = page;
            return View("~/Views/Orders/Order.cshtml", orders);
        }

        // ✅ View xác nhận đơn hàng
        public async Task<IActionResult> ConfirmPage(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null || order.OrderID == 0)
            {
                return NotFound();
            }
            return View("~/Views/Orders/Confirm.cshtml", order);
        }

        // ✅ Gửi xác nhận đơn hàng
        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var success = await _orderService.ConfirmOrderAsync(id);
            if (success)
            {
                TempData["Success"] = "Xác nhận đơn hàng thành công!";
                return RedirectToAction("Details", new { id });
            }

            TempData["Error"] = "Lỗi khi xác nhận đơn hàng!";
            return RedirectToAction("ConfirmPage", new { id });
        }
    }

    public class CartViewModel
    {
        public int CartID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}