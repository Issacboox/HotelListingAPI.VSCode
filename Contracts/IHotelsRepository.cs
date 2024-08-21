using HotelListingAPI.API.Data;

namespace HotelListingAPI.VSCode.Contracts
{
    public interface IHotelsRepository : IGenericRepository<Hotel>
    {
        Task<Hotel> GetHotelsDetails(int? id); 
    }
}