using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using LibraryMgmt.Data;
using LibraryMgmt.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using LibraryMgmt.PatchExamples;
using AutoMapper;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services
{
    public class BookStatusService : IBookStatusService
    {
        private readonly IBookStatusRepository _bookStatusRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public BookStatusService(IBookStatusRepository bookSatusRepository, DataContext context, IMapper mapper)
        {
            _bookStatusRepository = bookSatusRepository;
            _context = context;
            _mapper = mapper;
        }

        public OperationalResult<ICollection<BookStatusDto>> GetBookStatuses()
        {
            var bookStatuses = _mapper.Map<ICollection<BookStatusDto>>(_bookStatusRepository.GetAll());

            if (bookStatuses == null || bookStatuses.Count == 0)
                return OperationalResult<ICollection<BookStatusDto>>.Error("No book statuses found.");

            return OperationalResult<ICollection<BookStatusDto>>.Ok(bookStatuses);
        }
        public OperationalResult<BookStatusDto> GetBookStatusById(int bookStatusId)
        {
            var bookStatus = _mapper.Map<BookStatusDto>(_bookStatusRepository.GetBookStatusById(bookStatusId));

            if (bookStatus == null)
                return OperationalResult<BookStatusDto>.Error("No book status found.");

            return OperationalResult<BookStatusDto>.Ok(bookStatus);
        }
        public void CheckoutBook(int bookId, int studentId)
        {
            var checkoutDate = DateTime.Now;

            _context.Database.ExecuteSqlInterpolated(
                $"EXEC CheckoutBook {bookId}, {studentId}, {checkoutDate}");
        }

        public OperationalResult<BookReturnedDto> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState)
        {
            var bookStatus = _bookStatusRepository.GetBookStatusById(id);
            if (bookStatus == null)
                return OperationalResult<BookReturnedDto>.Error("Book not found.");

            if (patchDoc == null)
                return OperationalResult<BookReturnedDto>.Error("Invalid patch document.");

            PatchHelper.TryApplyPatch(patchDoc, bookStatus, modelState);

            if (!modelState.IsValid)
                return OperationalResult<BookReturnedDto>.Error("Patch validation failed.");

            var saved = _bookStatusRepository.Save();

            var dto = new BookReturnedDto
            {
                BookId = bookStatus.BookId,
                Title = bookStatus.Book?.Title ?? "Unknown",
                StudentName = $"{bookStatus.Student?.FirstName ?? "Unknown"} {bookStatus.Student?.LastName ?? ""}".Trim(),
                DateReturned = bookStatus.DateReturned ?? DateTime.MinValue
            };

            if (!saved)
                return OperationalResult<BookReturnedDto>.Error("Failed to save changes.");

            return OperationalResult<BookReturnedDto>.Ok(dto);
        }

        public OperationalResult<BookReturnedDto> ReturnBookByInt(int bookId)
        {
            var bookStatus = _bookStatusRepository.GetBookStatusById(bookId);

            
            if (bookStatus == null)
                return OperationalResult<BookReturnedDto>.Error("No matching checkout found", ErrorCode.NotFound);

            if (bookStatus.DateReturned.HasValue)
                return OperationalResult<BookReturnedDto>.Error("This book has already been returned.", ErrorCode.ValidationFailed);

            bookStatus.DateReturned = DateTime.UtcNow;

            var dto = new BookReturnedDto
            {
                BookId = bookStatus.BookId,
                Title = bookStatus.Book?.Title ?? "Unknown",
                StudentName = $"{bookStatus.Student?.FirstName ?? "Unknown"} {bookStatus.Student?.LastName ?? ""}".Trim(),
                DateReturned = bookStatus.DateReturned ?? DateTime.MinValue
            };

            if (!_bookStatusRepository.Save())
                return OperationalResult<BookReturnedDto>.Error("Something went wrong returning the book.", ErrorCode.SaveFailed);

            return OperationalResult<BookReturnedDto>.Ok(dto);
        }

        private int GeBookStatusBy(int studentId, int bookId)
        {
            var bookStatus = _context.BookStatuses
                .Where(bs => bs.StudentId == studentId && bs.BookId == bookId && bs.DateReturned == null).FirstOrDefault();

            return bookStatus.BookStatusId;

        }
    }
}
