using BookClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookClient.Services
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllOrdersAsync(int page = 1, int pageSize = 5, string search = "", string status = "");
        Task<Order> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderFromCartAsync(int cartId);
        Task<List<OrderItem>> GetOrderItemsFromCartAsync(int cartId);
    }
}