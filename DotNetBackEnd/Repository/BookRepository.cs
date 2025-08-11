using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMgmt.Repository
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context) : base(context) 
        {
            _context = context;
        }

        public Book GetBookById(int bookId)
        {
            return _context.Books.FirstOrDefault(bs => bs.BookId == bookId);
        }

        public Book GetBookByIsbn(string isbn)
        {
            return _context.Books.FirstOrDefault(bs => bs.Isbn == isbn);
        }

        public bool UpdateNoBooks(int bookId, int noCopies)
        {
            var book = _context.Books.FirstOrDefault(bs => bs.BookId == bookId);

            if(book == null)
            {
                return false;
            }

            book.NoCopies = book.NoCopies + noCopies;

            return Save();
        }

        public bool BookExists(string isbn)
        {
            return _context.Books.Any(b => b.Isbn == isbn);
        }

        public bool AddBook(Book book)
        {
            _context.Books.Add(book);

            return Save();
        }
    }
}
