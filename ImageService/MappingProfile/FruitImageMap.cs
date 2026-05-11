using AutoMapper;
using ImageService.Models.DTO;
using ImageService.Models;

namespace ImageService.MappingProfile
{
    public class FruitImageMap:Profile
    {
        public FruitImageMap()
        {
            CreateMap<NewImagesDTO, Images>().ReverseMap();
            CreateMap<UpdatedImagesDTO, Images>().ReverseMap();
        }
}
