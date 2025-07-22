using AdminUI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminUI.Services
{
    public interface IOrderService
    {
        Task<(List<Order> Orders, int TotalCount)> GetOrdersAsync(string searchTerm, string statusFilter, int page, int pageSize);
        Task<Order> GetOrderByIdAsync(int id);
    }
}