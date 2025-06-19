using AutoMapper;
using BookManagementApi.Models; 
using BookManagementApi.DTOs;   
public class BooksImgProfile : Profile
{
    public BooksImgProfile()
    {
        CreateMap<BooksImgCreateDto, BooksImg>();
        CreateMap<BooksImg, BooksImgDto>();
    }
}
