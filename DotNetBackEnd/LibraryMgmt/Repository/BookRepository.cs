using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Repository
{
    public class BookRepository : BaseRepository<Book>, IBookRepository
    {
        private readonly DataContext _context;

        public BookRepository(DataContext context) : base(context) 
        {
            _context = context;
        }

        public async Task<Book> GetBookById(int bookId)
        {
            return await _context.Books.FirstOrDefaultAsync(bs => bs.BookId == bookId);
        }

        public async Task<Book> GetBookByIsbn(string isbn)
        {
            return await _context.Books.FirstOrDefaultAsync(bs => bs.Isbn == isbn);
        }

        public async Task<bool> UpdateNoBooks(int bookId, int noCopies)
        {
            var book = await _context.Books.FirstOrDefaultAsync(bs => bs.BookId == bookId);

            if(book == null)
            {
                return false;
            }

            book.NoCopies = book.NoCopies + noCopies;

            return await SaveAsync();
        }

        public async Task<bool> BookExists(string isbn)
        {
            return _context.Books.Any(b => b.Isbn == isbn);
        }

        public async Task<Book> AddBook(Book book)
        {
            _context.Books.Add(book);

            var success = await SaveAsync();
            if (!success)
            {
                return null; 
            }
            return await GetBookByIsbn(book.Isbn);
        }
    }
}
