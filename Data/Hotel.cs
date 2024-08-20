
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListingAPI.API.Data
{
    public class Hotel
    {
        public int HotelId {get; set;}
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        
        [ForeignKey("CountryId")]
        public int CountryId { get; set; }
        public Country Country { get; set; }
    }
}




