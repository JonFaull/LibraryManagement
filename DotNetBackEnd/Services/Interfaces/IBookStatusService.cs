using LibraryMgmt.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookStatusService
    {
        OperationalResult<ICollection<BookStatus>> GetBookStatuses();
        OperationalResult<BookStatus> GetBookStatusById(int bookStatusId);
        void CheckoutBook(int bookId, int studentId);
        OperationalResult<object> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState);

        OperationalResult<object> ReturnBookByInt(int bookId);
    }
}
