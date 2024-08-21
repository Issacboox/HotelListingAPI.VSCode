using AutoMapper;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Models.Country;
using HotelListingAPI.VSCode.Models.Hotel;

namespace HotelListingAPI.VSCode.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();
            CreateMap<Hotel, HotelDto>().ReverseMap();
        }
    }
}