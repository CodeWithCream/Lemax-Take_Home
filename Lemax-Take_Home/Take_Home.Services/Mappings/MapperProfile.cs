using AutoMapper;
using Lemax_Take_Home.DTOs;
using NetTopologySuite.Geometries;
using Take_Home.Model;

namespace Lemax_Take_Home.Mappings
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Point, GeolocationDto>()
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.X))
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Y))
                .ReverseMap()
                    .ConstructUsing(src => new Point(src.Longitude, src.Latitude));

            CreateMap<Hotel, HotelDto>();
            CreateMap<CreateEditHotelDto, Hotel>()
                .ForMember(dest => dest.Geolocation, opt => opt.MapFrom(src => src.Geolocation));
        }
    }
}
