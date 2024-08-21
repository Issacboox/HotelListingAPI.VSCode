using HotelListingAPI.VSCode.Models.Country;

namespace HotelListingAPI.VSCode.Models.Hotel
{
    public class HotelDto : BaseHotelDto
    {
        public int HotelId { get; set; }
        public List<CountryDto> Countries { get; set; }
       
    }
}