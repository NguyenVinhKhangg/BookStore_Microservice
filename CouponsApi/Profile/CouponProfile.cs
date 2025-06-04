using AutoMapper;
using CouponsApi.DTOs;
using CouponsApi.Models;

namespace CouponsAPI.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupons, CouponReadDto>();
            CreateMap<CouponCreateDto, Coupons>();
            CreateMap<CouponUpdateDto, Coupons>();
            CreateMap<Coupons, CouponUpdateDto>();
        }
    }
}