using AutoMapper;
using HotelListing.API.Data;
using HotelListingAPI.API.Data;
using HotelListingAPI.VSCode.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Repository
{
    public class HotelRepository : GenericRepository<Hotel>, IHotelsRepository
    {
        private readonly HotelListingDbContext _context;
        private readonly IMapper _mapper;

        public HotelRepository(HotelListingDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public async Task<Hotel> GetHotelsDetails(int? id)
        {
            return await _context.Hotels.FirstOrDefaultAsync(q => q.HotelId == id);
        }

        // public string SomeLongRunningOperation()
        // {
        //     var hello = "hello ka" ;
        //     for (int i = 0; i < 1000; i++)
        //     {
        //         Console.Write($"Count {i}");
        //     }
        //     return hello;
        // }
    }
}
