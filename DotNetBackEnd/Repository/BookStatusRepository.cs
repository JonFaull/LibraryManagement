using LibraryMgmt.Models;
using LibraryMgmt.Data;

using Microsoft.EntityFrameworkCore.Query.Internal;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Repository
{
    public class BookStatusRepository : BaseRepository<BookStatus>, IBookStatusRepository
    {
        private readonly DataContext _context;

        public BookStatusRepository(DataContext context): base(context)
        {
            _context = context;
        }

        public ICollection<BookStatus> GetBookStatuses()
        {
            return _context.BookStatuses.ToList();
        }

        public BookStatus GetBookStatusById(int bookStatusId)
        {
            return _context.BookStatuses
                .Include(bs => bs.Book)
                .Include(bs => bs.Student)
                .FirstOrDefault(bs => bs.BookStatusId == bookStatusId);

        }

        public bool BookStatusExists(int bookStatusId)
        {
            return _context.BookStatuses.Any(bs => bs.BookStatusId == bookStatusId);
        }

        public void CheckoutBook(int bookId, int studentId)
        {

        }

        public bool ReturnBook(BookStatus bookStatus)
        {
            bookStatus.DateReturned = DateTime.Now;
            _context.BookStatuses.Update(bookStatus);
            return Save();
        }
    }
}
