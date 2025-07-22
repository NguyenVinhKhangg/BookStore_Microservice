using BookClient.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BookClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(HttpClient httpClient, ILogger<OrderService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Order>> GetAllOrdersAsync(int page = 1, int pageSize = 5, string search = "", string status = "")
        {
            var response = await _httpClient.GetAsync($"Orders?page={page}&pageSize={pageSize}&search={search}&status={status}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OrderListResult>(content);
            return result?.Orders ?? new List<Order>();
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Order>(content);
        }

        public async Task<Order> CreateOrderFromCartAsync(int cartId)
        {
            var response = await _httpClient.PostAsJsonAsync($"Orders/fromcart/{cartId}", new { });
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Order>(content);
        }

        public async Task<List<OrderItem>> GetOrderItemsFromCartAsync(int cartId)
        {
            var response = await _httpClient.GetAsync($"Cart/{cartId}/items");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<OrderItem>>(content);
        }

        // ✅ Xác nhận đơn hàng
        public async Task<bool> ConfirmOrderAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.PutAsync($"Orders/{orderId}/confirm", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xác nhận đơn hàng ID {orderId}");
                return false;
            }
        }

        private class OrderListResult
        {
            public List<Order> Orders { get; set; }
            public int TotalCount { get; set; }
        }
    }
}
