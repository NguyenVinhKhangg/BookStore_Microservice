using AutoMapper;
using BookImgManagementApi.DTOs;
using BookImgManagementApi.Models;

namespace BookImgManagementApi.Profiles
{
    public class BooksImgProfile : Profile
    {
        public BooksImgProfile()
        {
            CreateMap<BooksImg, BooksImgReadDto>();
            CreateMap<BooksImgCreateDto, BooksImg>();
            CreateMap<BooksImgUpdateDto, BooksImg>();
        }
    }
}
