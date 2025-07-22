using BookClient.Controllers;
using BookClient.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BookClient.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IHttpClientFactory httpClientFactory, ILogger<OrderService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("OrderAPI");
            _logger = logger;
        }

        public async Task<List<Order>> GetAllOrdersAsync(int page = 1, int pageSize = 5, string search = "", string status = "")
        {
            try
            {
                var response = await _httpClient.GetAsync($"Orders?Page={page}&PageSize={pageSize}&Search={search}&Status={status}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<PagedResult<Order>>(content);
                return result?.Items ?? new List<Order>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách đơn hàng");
                return new List<Order>();
            }
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Orders/{id}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(content);
                if (order != null)
                {
                    order.OrderItems = await GetOrderItemsFromCartAsync(order.CartID);
                }
                return order ?? new Order();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy đơn hàng với ID {id}");
                return new Order();
            }
        }

        public async Task<Order> CreateOrderFromCartAsync(int cartId)
        {
            try
            {
                var cartResponse = await _httpClient.GetAsync($"Carts/{cartId}");
                cartResponse.EnsureSuccessStatusCode();
                var cartContent = await cartResponse.Content.ReadAsStringAsync();
                var cartData = JsonConvert.DeserializeObject<CartViewModel>(cartContent);

                if (cartData == null) return new Order();

                var order = new Order
                {
                    CartID = cartId,
                    UserID = cartData.UserID.ToString(),
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    OrderItems = await GetOrderItemsFromCartAsync(cartId)
                };

                var orderJson = JsonConvert.SerializeObject(order);
                var orderResponse = await _httpClient.PostAsync("Orders", new StringContent(orderJson, System.Text.Encoding.UTF8, "application/json"));
                orderResponse.EnsureSuccessStatusCode();
                var orderContent = await orderResponse.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<Order>(orderContent) ?? order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tạo đơn hàng từ giỏ hàng với CartID {cartId}");
                return new Order();
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsFromCartAsync(int cartId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Carts/{cartId}/Items");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<OrderItem>>(content) ?? new List<OrderItem>();
                }
                return new List<OrderItem>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy mục đơn hàng từ giỏ hàng với CartID {cartId}");
                return new List<OrderItem>();
            }
        }
    }

    // Model để xử lý kết quả phân trang từ API
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
    }
}