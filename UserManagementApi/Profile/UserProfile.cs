using BussinessObject.DTO.UserDTO;
using UserManagementApi.DTOs;
using UserManagementApi.Models;

namespace UserManagementApi.Profile
{
    public class UserProfile : AutoMapper.Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber.ToString("D10")))
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<UpdateUserDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber.ToString("D10")))
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Password, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Password)))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<UpdateProfileDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber.ToString("D10")))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<RegisterDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber.ToString("D10")))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<Users, UserDTO>()
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Phonenumber, opt => opt.MapFrom(src => long.Parse(src.PhoneNumber ?? "0")))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleID))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));
        }
    }
}
