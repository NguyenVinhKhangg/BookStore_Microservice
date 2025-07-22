using AutoMapper;
using CouponsApi.DTOs;
using CouponsApi.Models;

namespace CouponsApi.Profiles
{
    public class CouponProfile : Profile
    {
        public CouponProfile()
        {
            CreateMap<Coupons, CouponReadDto>();
            CreateMap<CouponDTO, Coupons>();
            CreateMap<CouponUpdateDto, Coupons>();
        }
    }
}