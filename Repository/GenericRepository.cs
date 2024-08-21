
using HotelListing.API.Data;
using HotelListingAPI.VSCode.Contracts;
using Microsoft.EntityFrameworkCore;

namespace HotelListingAPI.VSCode.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HotelListingDbContext _context;

        //make to interact with database
        public GenericRepository(HotelListingDbContext context)
        {
            this._context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            // just using this AddAsync it will autometically be deduce ehat T is and which table entity should belong to
            await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _context.Set<T>().Remove(entity);

        }

        public async Task<bool> Exists(int id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            // mean go to database and get the DB set that is associated with T
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if (id is null)
            {
                return null;
            }
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}