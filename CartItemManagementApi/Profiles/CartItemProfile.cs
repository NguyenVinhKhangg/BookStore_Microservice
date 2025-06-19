using AutoMapper;
using CartItemManagementApi.DTOs;
using CartItemManagementApi.Models;

namespace CartItemManagementApi.Profiles
{
    public class CartItemProfile : Profile
    {
        public CartItemProfile()
        {
            CreateMap<CartItems, CartItemReadDto>();
            CreateMap<CartItemCreateDto, CartItems>();
            CreateMap<CartItemUpdateDto, CartItems>();
        }
    }
}
