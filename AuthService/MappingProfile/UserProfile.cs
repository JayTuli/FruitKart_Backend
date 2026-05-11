using AccountService.Models.DTO;
using AccountService.Models;
using AutoMapper;

namespace AccountService.MappingProfile
{
    public class UserProfile : Profile
    {
        public UserProfile() {
            CreateMap<LoginDTO, User>().ReverseMap();
            CreateMap<NewUserDTO, User>().ReverseMap();
            CreateMap<UpdateUserDTO, User>().ReverseMap();
        }
    }
}
