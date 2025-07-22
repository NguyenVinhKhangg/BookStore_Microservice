using OrdersManagementApi.DTOs;

namespace OrdersManagementApi.Services
{
    public interface IOrderService
    {
        // ✅ Basic CRUD operations
        Task<IEnumerable<OrderReadDto>> GetAllAsync();
        Task<OrderReadDto?> GetByIdAsync(int id);
        Task<OrderReadDto> CreateOrderAsync(OrderCreateDto orderDto);
       // Task<bool> UpdateOrderAsync(int id, OrderUpdateDto orderDto);
        Task<bool> DeleteOrderAsync(int id);
        
        // ✅ Checkout flow from cart
        Task<OrderReadDto> CheckoutFromCartAsync(CheckoutDto checkoutDto);

        // ✅ Admin order management
        Task<OrderReadDto> UpdateOrderStatusAsync(int orderId, UpdateOrderStatusDto statusDto, int adminUserId);
        Task<(IEnumerable<OrderSummaryDto> Orders, int TotalCount)> GetOrdersForAdminAsync(
            string? searchTerm = null,
            string? statusFilter = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int page = 1,
            int pageSize = 10);

        // ✅ User order management
        Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersByUserAsync(
            int userId,
            string? statusFilter = null,
            int page = 1,
            int pageSize = 10);

        // ✅ Legacy methods (để tương thích với code cũ)
        Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersAsync(
            string searchTerm,
            string statusFilter,
            int page,
            int pageSize);
        Task ConfirmAsync(int id);

        // ✅ Statistics and reporting
        Task<int> GetOrderCountAsync(string? statusFilter = null);
        Task<decimal> GetTotalRevenueAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IDictionary<string, int>> GetOrderStatusStatsAsync();
    }
}