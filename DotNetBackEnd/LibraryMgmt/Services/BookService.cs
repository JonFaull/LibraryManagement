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
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository bookRepository, DataContext context, IMapper mapper, ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationalResult<ICollection<BookDto>>> GetBooks()
        {
            _logger.LogInformation("Fetching all books...");

            var books = _mapper.Map<List<BookDto>>(await _bookRepository.GetAllAsync());

            if (books == null || books.Count == 0)
            {
                _logger.LogWarning("No books found in the database.");
                return OperationalResult<ICollection<BookDto>>.Error("No books found.");
            }

            _logger.LogInformation("Retrieved {Count} books.", books.Count);
            return OperationalResult<ICollection<BookDto>>.Ok(books);
        }

        public async Task<OperationalResult<BookDto>> GetBookById(int bookId)
        {
            _logger.LogInformation("Fetching book with ID: {BookId}", bookId);

            var book = _mapper.Map<BookDto>(await _bookRepository.GetBookById(bookId));

            if (book == null)
            {
                _logger.LogWarning("No book found with ID: {BookId}", bookId);
                return OperationalResult<BookDto>.Error("No book found with the given ID.");
            }

            _logger.LogInformation("Book found: {Title}", book.Title);
            return OperationalResult<BookDto>.Ok(book);
        }

        public async Task<OperationalResult<BookDto>> AddBook(AddBookDto bookDto)
        {
            _logger.LogInformation("Attempting to add book with ISBN: {Isbn}", bookDto.Isbn);

            var existingBook = await _bookRepository.GetBookByIsbn(bookDto.Isbn);

            if (existingBook != null)
            {
                _logger.LogInformation("Book with ISBN {Isbn} already exists. Updating copy count...", bookDto.Isbn);

                var updateSuccess = await _bookRepository.UpdateNoBooks(existingBook.BookId, bookDto.NoCopies);

                if (updateSuccess)
                {
                    _logger.LogInformation("Successfully updated book copies for ISBN: {Isbn}", bookDto.Isbn);
                    var updatedBook = await _bookRepository.GetBookByIsbn(bookDto.Isbn);
                    return OperationalResult<BookDto>.Ok(_mapper.Map<BookDto>(updatedBook));
                }

                _logger.LogError("Failed to update book copies for ISBN: {Isbn}", bookDto.Isbn);
                return OperationalResult<BookDto>.Error("Failed to update existing book.", ErrorCode.SaveFailed);
            }

            var addedBook = await _bookRepository.AddBook(_mapper.Map<Book>(bookDto));

            if (addedBook != null)
            {
                _logger.LogInformation("Successfully added new book: {Title}", addedBook.Title);
                return OperationalResult<BookDto>.Ok(_mapper.Map<BookDto>(addedBook));
            }

            _logger.LogError("Failed to add new book with ISBN: {Isbn}", bookDto.Isbn);
            return OperationalResult<BookDto>.Error("Failed to add new book.", ErrorCode.SaveFailed);
        }
    }
}
