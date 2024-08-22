using HotelListingAPI.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListingAPI.VSCode.Data.Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasData(
               new Country
               {
                   Id = 1,
                   Name = "Thailand",
                   ShortName = "TH"
               },
               new Country
               {
                   Id = 2,
                   Name = "United Kindom",
                   ShortName = "UK"
               },
               new Country
               {
                   Id = 3,
                   Name = "United State",
                   ShortName = "US"
               }
            );
        }
    }
}