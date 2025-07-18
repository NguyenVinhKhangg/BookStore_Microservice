using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using OrdersManagementApi.DTOs;

namespace OrdersManagementApi.ApiClients
{
    public class PaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:6001/api/payment"; // Giá trị mặc định

        public PaymentServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            if (string.IsNullOrEmpty(_baseUrl)) throw new ArgumentNullException(nameof(_baseUrl));
        }

        public async Task<bool> ProcessPaymentAsync(PaymentRequestDto paymentRequest)
        {
            var jsonContent = JsonSerializer.Serialize(paymentRequest);
            var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_baseUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<bool>(responseContent);
        }
    }
}