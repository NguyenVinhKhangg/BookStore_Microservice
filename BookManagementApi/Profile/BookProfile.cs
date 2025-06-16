using AutoMapper;
using BookManagementApi.Models;
using BookManagementApi.DTOs;

namespace BookManagementApi.Profiles
{
    public class BookProfile : Profile
    {
        public BookProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookCreateDto, Book>();
            CreateMap<BookUpdateDto, Book>();
        }
    }
}
