using OrdersManagementApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersManagementApi.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderReadDto>> GetAllAsync();
        Task<OrderReadDto> GetByIdAsync(int id);
        Task<OrderReadDto> AddAsync(OrderCreateDto order);
        Task UpdateAsync(int id, OrderUpdateDto order);
        Task DeleteAsync(int id);
    }
}