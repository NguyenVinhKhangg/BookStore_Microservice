using AdminUI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace AdminUI.Services
{
    public class OrderService : IOrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("OrderAPI");
        }

        public async Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(string searchTerm, string statusFilter, int page, int pageSize)
        {
            var queryString = $"Orders?searchTerm={searchTerm}&statusFilter={statusFilter}&page={page}&pageSize={pageSize}";
            var response = await _httpClient.GetAsync(queryString);
            if (!response.IsSuccessStatusCode)
            {
                return (new List<Order>(), 0);
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<(List<Order> Orders, int TotalCount)>(content);
            return result;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Orders/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Order>(content);
        }
    }
}