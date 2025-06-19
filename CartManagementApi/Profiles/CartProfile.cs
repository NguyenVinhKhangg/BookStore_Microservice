using AutoMapper;
using CartManagementApi.DTOs;
using CartManagementApi.Models;


namespace CartManagementApi.Profiles
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<Cart, CartReadDto>();
            CreateMap<CartCreateDto, Cart>();
            CreateMap<CartUpdateDto, Cart>();
        }
    }
}
