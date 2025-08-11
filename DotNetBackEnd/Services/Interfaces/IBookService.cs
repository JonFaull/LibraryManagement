using LibraryMgmt.Models;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookService
    {
        OperationalResult<ICollection<Book>> GetBooks();

        OperationalResult<Book> GetBookById(int bookId);

        OperationalResult<object> AddBook(Book book);

        bool UpdateAmountOfBook(int bookId, int noOfBooks);
    }
}
