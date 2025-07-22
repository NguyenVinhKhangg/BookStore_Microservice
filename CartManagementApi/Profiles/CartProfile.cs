using AutoMapper;
using CartManagementApi.DTOs;
using CartManagementApi.Models;

namespace CartManagementApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Cart mappings
            CreateMap<Cart, CartReadDto>();
            CreateMap<CartCreateDto, Cart>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());
            CreateMap<CartUpdateDto, Cart>()
                .ForMember(dest => dest.CartItems, opt => opt.Ignore());

            // CartItem mappings
            CreateMap<CartItem, CartItemReadDto>();
            CreateMap<CartItemCreateDto, CartItem>()
                .ForMember(dest => dest.AddedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
            CreateMap<CartItemUpdateDto, CartItem>()
                .ForMember(dest => dest.CartItemID, opt => opt.Ignore())
                .ForMember(dest => dest.AddedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom((src, dest) => src.Price ?? dest.Price)); // ✅ Update price only if provided
        }
    }
}