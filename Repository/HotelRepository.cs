using HotelListing.API.Data;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Repository
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListingDbContext _context;

        public HotelRepository(HotelListingDbContext context) : base(context)
        {
            this._context = context;
        }

        public async Task<Hotel> GetHotelsDetails(int? id)
        {
            return await _context.Hotels.FirstOrDefaultAsync(q => q.HotelId == id);
        }
    }
}
