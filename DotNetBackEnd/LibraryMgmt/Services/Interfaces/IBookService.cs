using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookService
    {
        Task<OperationalResult<ICollection<BookDto>>> GetBooks();

        Task<OperationalResult<BookDto>> GetBookById(int bookId);

        Task<OperationalResult<BookDto>> AddBook(AddBookDto book);
    }
}
