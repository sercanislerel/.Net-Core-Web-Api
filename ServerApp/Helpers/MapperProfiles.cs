using System.Linq;
using AutoMapper;
using ServerApp.Dtos;
using ServerApp.Models;

namespace ServerApp.Helpers
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
         CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.ImageUrl, opt =>
                     opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Name))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DataOfBirth.CalculateAge()));
         CreateMap<User, UserForDetailsDto>()
            .ForMember(dest => dest.ProfileImageUrl, opt =>
                     opt.MapFrom(src => src.Images.FirstOrDefault(i => i.IsProfile).Name))
            .ForMember(dest => dest.Images, opt =>
                     opt.MapFrom(src => src.Images.Where(i => !i.IsProfile).ToList()))
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DataOfBirth.CalculateAge()));
            ;
         CreateMap<Image, ImagesForDetails>();
         CreateMap<UserForUpdateDto,User>();


        }
    }
}