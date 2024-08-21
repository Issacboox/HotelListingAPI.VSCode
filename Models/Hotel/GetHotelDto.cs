using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListingAPI.VSCode.Models.Hotel
{
    public class GetHotelDto : BaseHotelDto
    {
        public int HotelId { get; set; }
    }
}