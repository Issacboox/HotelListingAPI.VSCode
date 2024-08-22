using HotelListingAPI.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.VSCode.Data.Configurations
{
    public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
    {
        void IEntityTypeConfiguration<Hotel>.Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    HotelId = 1,
                    Name = "Lorem yipsum",
                    Address = "Bangkok, Thailand",
                    CountryId = 1,
                    Rating = 5
                },
                 new Hotel
                 {
                     HotelId = 2,
                     Name = "Lorem yipsum 1",
                     Address = "America",
                     CountryId = 3,
                     Rating = 5
                 },
                 new Hotel
                 {
                     HotelId = 3,
                     Name = "Lorem yipsum 2",
                     Address = "England",
                     CountryId = 2,
                     Rating = 5
                 }
            );
        }
    }
}