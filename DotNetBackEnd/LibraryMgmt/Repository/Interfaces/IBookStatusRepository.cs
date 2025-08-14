using LibraryMgmt.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IBookStatusRepository : IRepository<BookStatus>
    {
        Task<BookStatus> GetBookStatusById(int bookStatusId);

        Task<bool> BookStatusExists(int bookStatusId);

        Task<bool> ReturnBook(BookStatus bookStatus);
    }
}
