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
            // Country
            CreateMap<Country, CreateCountryDto>().ReverseMap();
            CreateMap<Country, GetCountryDto>().ReverseMap();
            CreateMap<Country, CountryDto>().ReverseMap();
            CreateMap<Country, UpdateCountryDto>().ReverseMap();


            // Hotel
            CreateMap<Hotel, HotelDto>().ReverseMap();
            CreateMap<Hotel, GetHotelDto>().ReverseMap();
            CreateMap<Hotel, CreateHotelDto>().ReverseMap();
            CreateMap<Hotel, UpdateHotelDto>().ReverseMap();
        }
    }
}