using AutoMapper;
using OrdersManagementApi.DTOs;
using OrdersManagementApi.Models;

namespace OrdersManagementApi.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderUpdateDto, Order>();
        }
    }
}