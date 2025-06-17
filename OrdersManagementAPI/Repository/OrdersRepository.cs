using BookManagementApi.Data;
using BookManagementApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagementApi.Repository
{
    public class OrdersRepository : IOrdersRepository
    {
        private readonly BookStoreContext _context;

        public OrdersRepository(BookStoreContext context)
        {
            _context = context;
        }

        public async Task<Orders> GetByIdAsync(int orderId)
        {
            return await _context.Orders.FindAsync(orderId) ?? throw new Exception("Order not found"); // Thay null bằng exception
        }

        public async Task<List<Orders>> GetByUserIdAsync(int userId)
        {
            var orders = await _context.Orders.Where(o => o.UserId == userId).ToListAsync();
            return orders.Any() ? orders : throw new Exception("No orders found for user");
        }

        public async Task AddAsync(Orders order)
        {
            await _context.Orders.AddAsync(order);
        }

        public async Task<bool> UpdateAsync(Orders order)
        {
            _context.Orders.Update(order);
            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = status;
            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> CancelAsync(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null) return false;
            order.OrderStatus = "Cancelled";
            return await SaveChangesAsync() > 0;
        }

        public async Task<OrderDetails> GetOrderDetailByIdAsync(int orderDetailId)
        {
            return await _context.OrderDetails.FindAsync(orderDetailId) ?? throw new Exception("Order detail not found");
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}