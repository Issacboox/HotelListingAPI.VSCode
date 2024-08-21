using HotelListing.API.Data;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Contracts;
using Microsoft.EntityFrameworkCore;
namespace HotelListingAPI.VSCode.Repository
{
    // คลาส CountryRepository ที่สืบทอดจาก GenericRepository<Country>
    // และใช้ interface ICountriesRepository เพื่อจัดการกับข้อมูลประเทศโดยเฉพาะ
    public class CountryRepository : GenericRepository<Country>, ICountriesRepository
    {
        private readonly HotelListingDbContext _context;

        // คอนสตรัคเตอร์ที่รับ HotelListingDbContext และส่งไปให้คลาสแม่ (GenericRepository)
        public CountryRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
            // ไม่มีการเพิ่มการทำงานใหม่เพิ่มเติมที่นี่ แค่ส่ง context ไปยังคลาสแม่
        }

        public async Task<Country> GetDetails(int? id)
        {
            return await _context.Countries.Include(q => q.Hotels).FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
