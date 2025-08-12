using LibraryMgmt.Models;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book> GetBookById(int bookId);

        Task<Book> GetBookByIsbn(string isbn);

        Task<bool> BookExists(string isbn);

        Task<Book> AddBook(Book book);

        Task<bool> UpdateNoBooks(int bookId, int noCopies);
    }
}
