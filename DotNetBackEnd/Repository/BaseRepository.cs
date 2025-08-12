using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Data;
using Microsoft.EntityFrameworkCore;


namespace LibraryMgmt.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly DataContext _context;

        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public virtual async Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async virtual Task<bool> SaveAsync()
        {
            var saved = await _context.SaveChangesAsync();
            return saved > 0 ? true : false;
        }
    }
}
