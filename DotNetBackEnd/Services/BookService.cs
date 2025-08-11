using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;

namespace LibraryMgmt.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly DataContext _context;

        public BookService(IBookRepository bookRepository, DataContext context)
        {
            _bookRepository = bookRepository;
            _context = context;
        }

        public OperationalResult<ICollection<Book>> GetBooks()
        {
            var books = _bookRepository.GetAll();

            if (books == null || books.Count == 0)
                return OperationalResult<ICollection<Book>>.Error("No books found.");

            return OperationalResult<ICollection<Book>>.Ok(books);
        }

        public OperationalResult<Book> GetBookById(int bookId)
        {
            var book = _bookRepository.GetBookById(bookId); 

            if (book == null)
                return OperationalResult<Book>.Error("No book found with the given ID.");

            return OperationalResult<Book>.Ok(book);
        }

        public OperationalResult<object> AddBook(Book book)
        {
            var existingBook = _bookRepository.GetBookByIsbn(book.Isbn);

            if (existingBook != null)
            {
                var updateSuccess = _bookRepository.UpdateNoBooks(existingBook.BookId, book.NoCopies);

                if (updateSuccess)
                    return OperationalResult<object>.Ok("Book updated successfully.");
                else
                    return OperationalResult<object>.Error("Failed to update existing book.", ErrorCode.SaveFailed);
            }

            var addSuccess = _bookRepository.AddBook(book);

            if (addSuccess)
                return OperationalResult<object>.Ok("Book added successfully.");
            else
                return OperationalResult<object>.Error("Failed to add new book.", ErrorCode.SaveFailed);
        }


        public bool UpdateAmountOfBook(int bookId, int noOfBooks)
        {
            return false;
        }
    }
}
