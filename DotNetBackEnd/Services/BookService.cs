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

        public async Task<OperationalResult<ICollection<BookDto>>> GetBooks()
        {
            var books = _mapper.Map<List<BookDto>>(await _bookRepository.GetAllAsync());

            if (books == null || books.Count == 0)
                return OperationalResult<ICollection<BookDto>>.Error("No books found.");

            return OperationalResult<ICollection<BookDto>>.Ok(books);
        }

        public async Task<OperationalResult<BookDto>> GetBookById(int bookId)
        {
            var book = _mapper.Map<BookDto>(await _bookRepository.GetBookById(bookId)); 

            if (book == null)
                return OperationalResult<BookDto>.Error("No book found with the given ID.");

            return OperationalResult<BookDto>.Ok(book);
        }

        public async Task<OperationalResult<BookDto>> AddBook(BookDto bookDto)
        {
            var existingBook = await _bookRepository.GetBookByIsbn(bookDto.Isbn);

            if (existingBook != null)
            {
                var updateSuccess = await _bookRepository.UpdateNoBooks(existingBook.BookId, bookDto.NoCopies);

                if (updateSuccess)
                {
                    var updatedBook = await _bookRepository.GetBookByIsbn(bookDto.Isbn);
                    return OperationalResult<BookDto>.Ok(_mapper.Map<BookDto>(updatedBook));
                }

                return OperationalResult<BookDto>.Error("Failed to update existing book.", ErrorCode.SaveFailed);
            }

            var addedBook = await _bookRepository.AddBook(_mapper.Map<Book>(bookDto));

            if (addedBook != null)
                return OperationalResult<BookDto>.Ok(_mapper.Map<BookDto>(addedBook));
            else
                return OperationalResult<BookDto>.Error("Failed to add new book.", ErrorCode.SaveFailed);
        }
    }
}
