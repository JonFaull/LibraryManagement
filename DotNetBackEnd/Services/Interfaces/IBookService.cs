using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookService
    {
        OperationalResult<ICollection<BookDto>> GetBooks();

        OperationalResult<BookDto> GetBookById(int bookId);

        OperationalResult<object> AddBook(BookDto book);

        bool UpdateAmountOfBook(int bookId, int noOfBooks);
    }
}
