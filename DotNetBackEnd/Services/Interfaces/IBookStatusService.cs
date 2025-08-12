using LibraryMgmt.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IBookStatusService
    {
        Task<OperationalResult<ICollection<BookStatusDto>>> GetBookStatuses();
        Task<OperationalResult<BookStatusDto>> GetBookStatusById(int bookStatusId);
        Task<OperationalResult<bool>> CheckoutBookAsync(int bookId, int studentId);
        Task<OperationalResult<BookReturnedDto>> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState);

        Task<OperationalResult<BookReturnedDto>> ReturnBookByInt(int bookId);
    }
}
