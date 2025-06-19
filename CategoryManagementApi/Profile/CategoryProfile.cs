using AutoMapper;
using CategoryManagementApi.DTOs;
using CategoryManagementApi.Models;

namespace CategoryManagementApi.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<CreateCategoryDTO, Category>();
            CreateMap<UpdateCategoryDTO, Category>();
        }
    }
}