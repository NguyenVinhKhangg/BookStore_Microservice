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
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber))
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<UpdateUserDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber))
                .ForMember(dest => dest.RoleID, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Password, opt => opt.Condition(src => !string.IsNullOrEmpty(src.Password)))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<UpdateProfileDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            CreateMap<RegisterDTO, Users>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Phonenumber))
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));

            // ✅ FIXED: Cập nhật mapping để bao gồm tất cả fields cần thiết
            CreateMap<Users, UserDTO>()
                .ForMember(dest => dest.Fullname, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Phonenumber, opt => opt.MapFrom(src => src.PhoneNumber ?? ""))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleID))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role != null ? src.Role.RoleName : "Unknown"))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.IsDeactivated, opt => opt.MapFrom(src => src.DeactivatedStatus)) // ✅ Fix logic
                .ForMember(dest => dest.BirthDay, opt => opt.MapFrom(src => src.BirthDay));
        }
    }
}