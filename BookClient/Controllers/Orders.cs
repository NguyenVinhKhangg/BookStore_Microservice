using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using BookClient.Models;

namespace BookClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrderController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OrderAPI");
        }

        public async Task<IActionResult> Index(int cartId)
        {
            var response = await _httpClient.GetAsync($"Carts/{cartId}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var cartData = JsonConvert.DeserializeObject<CartViewModel>(content);
            if (cartData == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                CartID = cartId,
                UserID = cartData.UserID.ToString(),
                OrderItems = await GetOrderItemsFromApi(cartId)
            };

            return View("~/Views/Orders/Order.cshtml", order);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var content = await response.Content.ReadAsStringAsync();
            var order = JsonConvert.DeserializeObject<Order>(content);
            if (order == null)
            {
                return NotFound();
            }

            order.OrderItems = await GetOrderItemsFromApi(order.CartID);
            return View("~/Views/Orders/Order.cshtml", order);
        }

        public async Task<IActionResult> MyOrders()
        {
            var response = await _httpClient.GetAsync("Orders");
            if (!response.IsSuccessStatusCode)
            {
                return View("~/Views/Orders/Order.cshtml", new List<Order>());
            }

            var content = await response.Content.ReadAsStringAsync();
            var orders = JsonConvert.DeserializeObject<List<Order>>(content) ?? new List<Order>();
            return View("~/Views/Orders/Order.cshtml", orders);
        }

        private async Task<List<OrderItem>> GetOrderItemsFromApi(int cartId)
        {
            var response = await _httpClient.GetAsync($"Carts/{cartId}/Items");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<OrderItem>>(content);
                return items ?? new List<OrderItem>();
            }
            return new List<OrderItem>();
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