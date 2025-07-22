using OrdersManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersManagementApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderReadDto>> GetAllAsync();
        Task<OrderReadDto> GetByIdAsync(int id);
        Task<OrderReadDto> AddAsync(OrderCreateDto orderDto);
        Task UpdateAsync(int id, OrderUpdateDto orderDto);
        Task DeleteAsync(int id);
        Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersAsync(string searchTerm, string statusFilter, int page, int pageSize);
    }
}