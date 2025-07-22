using AutoMapper;
using ReviewsApi.DTOs;
using ReviewsApi.Models;

namespace ReviewsApi.Profiles
{
    public class ReviewProfile : Profile
    {
        public ReviewProfile()
        {
            CreateMap<Review, ReviewReadDto>().ReverseMap();
            CreateMap<ReviewCreateDto, Review>();
            CreateMap<ReviewUpdateDto, Review>();
        }
    }
}