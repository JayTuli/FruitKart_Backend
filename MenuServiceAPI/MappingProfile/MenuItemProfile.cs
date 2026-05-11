using AutoMapper;
using MenuServiceAPI.Models;
using MenuServiceAPI.Models.DTO;

namespace MenuServiceAPI.MappingProfile
{
    public class MenuItemProfile
    {
        CreateMap<MenuItem, MenuItemDTO>();
 
            CreateMap<MenuItemCreateDTO, MenuItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ImagePublicId, opt => opt.Ignore())
                .ForMember(dest => dest.File, opt => opt.Ignore());

        CreateMap<MenuItemUpdateDTO, MenuItem>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.ImagePublicId, opt => opt.Ignore())
                .ForMember(dest => dest.File, opt => opt.Ignore());
    }
}