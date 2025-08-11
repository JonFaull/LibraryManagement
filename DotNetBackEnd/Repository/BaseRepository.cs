using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Data;


namespace LibraryMgmt.Repository
{
    public class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly DataContext _context;

        public BaseRepository(DataContext context)
        {
            _context = context;
        }

        public virtual ICollection<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public virtual bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
