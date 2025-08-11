using LibraryMgmt.Models;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Book GetBookById(int bookId);

        Book GetBookByIsbn(string isbn);

        bool BookExists(string isbn);

        bool AddBook(Book book);

        bool UpdateNoBooks(int bookId, int noCopies);
    }
}
