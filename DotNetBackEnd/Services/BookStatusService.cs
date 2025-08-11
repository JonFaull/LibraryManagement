using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using LibraryMgmt.Data;
using LibraryMgmt.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.PatchExamples;
using System.Net;

namespace LibraryMgmt.Services
{
    public class BookStatusService : IBookStatusService
    {
        private readonly IBookStatusRepository _bookStatusRepository;
        private readonly DataContext _context;

        public BookStatusService(IBookStatusRepository bookSatusRepository, DataContext context)
        {
            _bookStatusRepository = bookSatusRepository;
            _context = context;
        }

        public OperationalResult<ICollection<BookStatus>> GetBookStatuses()
        {
            var bookStatuses = _bookStatusRepository.GetAll();

            if (bookStatuses == null || bookStatuses.Count == 0)
                return OperationalResult<ICollection<BookStatus>>.Error("No book statuses found.");

            return OperationalResult<ICollection<BookStatus>>.Ok(bookStatuses);
        }

        public OperationalResult<BookStatus> GetBookStatusById(int bookStatusId)
        {
            var bookStatus = _bookStatusRepository.GetBookStatusById(bookStatusId);

            if (bookStatus == null)
                return OperationalResult<BookStatus>.Error("No book status found.");

            return OperationalResult<BookStatus>.Ok(bookStatus);
        }
        public void CheckoutBook(int bookId, int studentId)
        {
            var checkoutDate = DateTime.Now;

            _context.Database.ExecuteSqlInterpolated(
                $"EXEC CheckoutBook {bookId}, {studentId}, {checkoutDate}");
        }

        public OperationalResult<object> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState)
        {
            var bookStatus = _bookStatusRepository.GetBookStatusById(id);
            if (bookStatus == null)
                return OperationalResult<object>.Error("Book not found.");

            if (patchDoc == null)
                return OperationalResult<object>.Error("Invalid patch document.");

            PatchHelper.TryApplyPatch(patchDoc, bookStatus, modelState);

            if (!modelState.IsValid)
                return OperationalResult<object>.Error("Patch validation failed.");

            var saved = _bookStatusRepository.Save();
            if (!saved)
                return OperationalResult<object>.Error("Failed to save changes.");

            return OperationalResult<object>.Ok("Book returned successfully :)");
        }

        public OperationalResult<object> ReturnBookByInt(int bookId)
        {
            var bookStatus = _bookStatusRepository.GetBookStatusById(bookId);
            if (bookStatus == null)
                return OperationalResult<object>.Error("No matching checkout found", ErrorCode.NotFound);

            if (bookStatus.DateReturned.HasValue)
                return OperationalResult<object>.Error("This book has already been returned.", ErrorCode.ValidationFailed);

            bookStatus.DateReturned = DateTime.Today;

            if (!_bookStatusRepository.Save())
                return OperationalResult<object>.Error("Something went wrong returning the book.", ErrorCode.SaveFailed);

            return OperationalResult<object>.Ok($"Book returned successfully on {DateTime.Today:yyyy-MM-dd}");
        }
    }
}
