// ✅ Đã sửa OrderService.cs (API backend) để không phân biệt hoa thường khi lọc Status
using AutoMapper;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Models;
using OrdersManagementApi.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrdersManagementApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderReadDto>>(orders) ?? new List<OrderReadDto>();
        }

        public async Task<OrderReadDto> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            return _mapper.Map<OrderReadDto>(order) ?? new OrderReadDto();
        }

        public async Task<OrderReadDto> AddAsync(OrderCreateDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            var addedOrder = await _repository.AddAsync(order);
            return _mapper.Map<OrderReadDto>(addedOrder) ?? new OrderReadDto();
        }

        public async Task UpdateAsync(int id, OrderUpdateDto orderDto)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order != null)
            {
                _mapper.Map(orderDto, order);
                await _repository.UpdateAsync(order);
            }
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<(IEnumerable<OrderReadDto> Orders, int TotalCount)> GetOrdersAsync(string searchTerm, string statusFilter, int page, int pageSize)
        {
            var orders = await _repository.GetAllAsync();
            var filteredOrders = orders
                .Where(o => string.IsNullOrEmpty(searchTerm) || o.OrderID.ToString().Contains(searchTerm) || o.UserID.ToString().Contains(searchTerm))
                .Where(o => string.IsNullOrEmpty(statusFilter) || o.Status.ToLower() == statusFilter.ToLower());

            var totalCount = filteredOrders.Count();
            var pagedOrders = filteredOrders
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return (_mapper.Map<IEnumerable<OrderReadDto>>(pagedOrders), totalCount);
        }

        public async Task ConfirmAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException();

            order.Status = "Confirmed";
            await _repository.UpdateAsync(order);
        }
    }
}