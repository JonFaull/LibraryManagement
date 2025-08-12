using AutoMapper;
using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BookService(IBookRepository bookRepository, DataContext context, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _context = context;
            _mapper = mapper;
        }

        public OperationalResult<ICollection<BookDto>> GetBooks()
        {
            var books = _mapper.Map<List<BookDto>>(_bookRepository.GetAll());

            if (books == null || books.Count == 0)
                return OperationalResult<ICollection<BookDto>>.Error("No books found.");

            return OperationalResult<ICollection<BookDto>>.Ok(books);
        }

        public OperationalResult<BookDto> GetBookById(int bookId)
        {
            var book = _mapper.Map<BookDto>(_bookRepository.GetBookById(bookId)); 

            if (book == null)
                return OperationalResult<BookDto>.Error("No book found with the given ID.");

            return OperationalResult<BookDto>.Ok(book);
        }

        public OperationalResult<object> AddBook(BookDto book)
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

            var addSuccess = _bookRepository.AddBook(_mapper.Map<Book>(book));

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
