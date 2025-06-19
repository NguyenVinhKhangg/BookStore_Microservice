using BookManagementApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagementApi.Repository
{
    public interface IOrdersRepository
    {
        Task<Orders> GetByIdAsync(int orderId);
        Task<List<Orders>> GetByUserIdAsync(int userId);
        Task AddAsync(Orders order);
        Task<bool> UpdateAsync(Orders order);
        Task<bool> UpdateStatusAsync(int orderId, string status);
        Task<bool> CancelAsync(int orderId);
        Task<OrderDetails> GetOrderDetailByIdAsync(int orderDetailId);
        Task<int> SaveChangesAsync(); // Cập nhật kiểu trả về
    }
}