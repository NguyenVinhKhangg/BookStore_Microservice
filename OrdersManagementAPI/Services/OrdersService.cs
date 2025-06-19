using BookManagementApi.Data;
using BookManagementApi.DTOs;
using BookManagementApi.Models;
using BookManagementApi.Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookManagementApi.Services
{
    public class OrdersService
    {
        private readonly IOrdersRepository _ordersRepository;

        public OrdersService(IOrdersRepository ordersRepository)
        {
            _ordersRepository = ordersRepository;
        }

        public async Task<OrdersDTO> GetOrderByIdAsync(int orderId)
        {
            var order = await _ordersRepository.GetByIdAsync(orderId);
            return new OrdersDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                Note = order.Note,
                PaymentId = order.PaymentId,
                ShippingId = order.ShippingId
            };
        }

        public async Task<List<OrdersDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _ordersRepository.GetByUserIdAsync(userId);
            return orders.Select(o => new OrdersDTO
            {
                OrderId = o.OrderId,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.OrderStatus,
                Note = o.Note,
                PaymentId = o.PaymentId,
                ShippingId = o.ShippingId
            }).ToList();
        }

        public async Task<OrdersDTO> CreateOrderAsync(OrdersDTO dto)
        {
            var order = new Orders
            {
                UserId = dto.UserId,
                OrderDate = DateTime.Now,
                TotalAmount = dto.TotalAmount,
                OrderStatus = dto.OrderStatus,
                Note = dto.Note,
                PaymentId = dto.PaymentId,
                ShippingId = dto.ShippingId
            };
            await _ordersRepository.AddAsync(order);
            await _ordersRepository.SaveChangesAsync();

            dto.OrderId = order.OrderId;
            return dto;
        }

        public async Task<bool> UpdateStatusOrderAsync(int orderId, string status)
        {
            return await _ordersRepository.UpdateStatusAsync(orderId, status);
        }

        public async Task<bool> UpdateOrderAsync(int orderId, OrdersDTO dto)
        {
            var order = await _ordersRepository.GetByIdAsync(orderId);
            order.UserId = dto.UserId;
            order.TotalAmount = dto.TotalAmount;
            order.OrderStatus = dto.OrderStatus;
            order.Note = dto.Note;
            order.PaymentId = dto.PaymentId;
            order.ShippingId = dto.ShippingId;

            return await _ordersRepository.UpdateAsync(order);
        }

        public async Task<bool> CancelOrderAsync(int orderId)
        {
            return await _ordersRepository.CancelAsync(orderId);
        }

        public async Task<OrderDetailsDTO> GetOrderDetailByIdAsync(int orderDetailId)
        {
            var orderDetail = await _ordersRepository.GetOrderDetailByIdAsync(orderDetailId);
            return new OrderDetailsDTO
            {
                OrderDetailId = orderDetail.OrderDetailId,
                OrderId = orderDetail.OrderId,
                BookId = orderDetail.BookId,
                Quantity = orderDetail.Quantity,
                UnitPrice = orderDetail.UnitPrice,
                Discount = orderDetail.Discount
            };
        }
    }
}