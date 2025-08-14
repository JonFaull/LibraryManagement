using LibraryMgmt.Models;
using LibraryMgmt.Data;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Repository
{
    public class BookStatusRepository : BaseRepository<BookStatus>, IBookStatusRepository
    {
        public BookStatusRepository(DataContext context): base(context)
        {
        }

        public async Task<ICollection<BookStatus>> GetBookStatuses()
        {
            return await _context.BookStatuses.ToListAsync();
        }

        public async Task<BookStatus> GetBookStatusById(int bookStatusId)
        {
            return await _context.BookStatuses
                .Include(bs => bs.Book)
                .Include(bs => bs.Student)
                .FirstOrDefaultAsync(bs => bs.BookStatusId == bookStatusId);

        }

        public async Task<bool> BookStatusExists(int bookStatusId)
        {
            return await _context.BookStatuses.AnyAsync(bs => bs.BookStatusId == bookStatusId);
        }

        public async Task<bool> ReturnBook(BookStatus bookStatus)
        {
            bookStatus.DateReturned = DateTime.Now;
            var returned = _context.BookStatuses.Update(bookStatus);

            if (returned == null)
            {
                return false;
            }
            return await SaveAsync();
        }
    }
}
