using OrdersManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersManagementApi.Repository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order> GetByIdAsync(int id);
        Task<Order> AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(int id);
    }
}