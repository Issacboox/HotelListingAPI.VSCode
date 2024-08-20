using HotelListingAPI.API.Data;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.API.Data
{
    public class HotelListingDbContext : DbContext
    {
        public HotelListingDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Country> Countries { get; set; }

        protected override void OnModeCreating(ModelBuilder modelBuilder)
        {
            base.OnModeCreating(modelBuilder)
        }
    }
}
