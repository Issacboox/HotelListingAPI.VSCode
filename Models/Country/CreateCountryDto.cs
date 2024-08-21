using System.ComponentModel.DataAnnotations;

namespace HotelListingAPI.VSCode.Models.Country
{
    public class CreateCountryDto
    {
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}