using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Data;
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
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasData(
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

            modelBuilder.Entity<Hotel>().HasData(
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
