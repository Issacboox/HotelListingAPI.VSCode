using HotelListingAPI.API.Data;

namespace HotelListingAPI.VSCode.Contracts
{
    // <T> Represent a class so that would represent data objects in the forms of country and hotel
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id);
        Task<List<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task DeleteAsync(int id);
        Task UpdateAsync(T entity);
        Task<bool> Exists(int id);
    }

}