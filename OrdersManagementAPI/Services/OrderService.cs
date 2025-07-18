using AutoMapper;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Models;
using OrdersManagementApi.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrdersManagementApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<OrderReadDto>> GetAllAsync()
        {
            var orders = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<OrderReadDto>>(orders);
        }

        public async Task<OrderReadDto> GetByIdAsync(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            return _mapper.Map<OrderReadDto>(order);
        }

        public async Task<OrderReadDto> AddAsync(OrderCreateDto orderDto)
        {
            var order = _mapper.Map<Order>(orderDto);
            var addedOrder = await _repository.AddAsync(order);
            return _mapper.Map<OrderReadDto>(addedOrder);
        }

        public async Task UpdateAsync(int id, OrderUpdateDto orderDto)
        {
            var order = await _repository.GetByIdAsync(id);
            _mapper.Map(orderDto, order);
            await _repository.UpdateAsync(order);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}