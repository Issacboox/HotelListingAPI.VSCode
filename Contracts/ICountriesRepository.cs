using HotelListingAPI.API.Data;

namespace HotelListingAPI.VSCode.Contracts
{
    public interface ICountriesRepository :  IGenericRepository<Country>
    {
       Task<Country> GetDetails(int? id); 
    }
}