using LibraryMgmt.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IBookStatusRepository : IRepository<BookStatus>
    {
        BookStatus GetBookStatusById(int bookStatusId);

        bool BookStatusExists(int bookStatusId);

        void CheckoutBook(int bookId, int studentId);

        bool ReturnBook(BookStatus bookStatus);
    }
}
