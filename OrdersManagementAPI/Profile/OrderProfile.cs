using AutoMapper;
using BookManagementApi.DTOs;
using BookManagementApi.Models;

namespace BookManagementApi.Profile
{
    public class OrderProfile : AutoMapper.Profile
    {
        public OrderProfile()
        {
            CreateMap<Orders, OrdersDTO>().ReverseMap();
            CreateMap<OrderDetails, OrderDetailsDTO>().ReverseMap();
        }
    }
}