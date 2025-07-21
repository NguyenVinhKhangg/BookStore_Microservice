using Newtonsoft.Json;
using AdminUI.Models; // Nếu bạn đã copy model vào đây
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdminUI.Controllers
{
    [Route("Orders")] // Thêm route gốc cho controller
    public class OrderManagementController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderManagementController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("OrderAPI");
        }

        [HttpGet("Index")] // Định nghĩa rõ route cho action
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("Orders");
            if (!response.IsSuccessStatusCode)
            {
                return View(new List<Order>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(content);
            return View(orders ?? new List<Order>());
        }

        [HttpGet("Details/{id}")] // Định nghĩa route cho xem chi tiết đơn hàng
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(content);
            return View(order);
        }
    }
}