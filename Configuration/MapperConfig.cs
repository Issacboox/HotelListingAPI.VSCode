using AutoMapper;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Models.Country;

namespace HotelListingAPI.VSCode.Configuration
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
        }
    }
}