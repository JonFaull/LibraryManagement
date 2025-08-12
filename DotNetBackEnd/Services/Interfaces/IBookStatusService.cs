using LibraryMgmt.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookStatusService
    {
        OperationalResult<ICollection<BookStatusDto>> GetBookStatuses();
        OperationalResult<BookStatusDto> GetBookStatusById(int bookStatusId);
        void CheckoutBook(int bookId, int studentId);
        OperationalResult<BookReturnedDto> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState);

        OperationalResult<BookReturnedDto> ReturnBookByInt(int bookId);
    }
}
