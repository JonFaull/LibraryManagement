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
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data;
using Microsoft.Data.SqlClient;

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

        public async Task<OperationalResult<ICollection<BookStatusDto>>> GetBookStatuses()
        {
            var bookStatuses = _mapper.Map<ICollection<BookStatusDto>>(await _bookStatusRepository.GetAllAsync());

            if (bookStatuses == null || bookStatuses.Count == 0)
                return OperationalResult<ICollection<BookStatusDto>>.Error("No book statuses found.");

            return OperationalResult<ICollection<BookStatusDto>>.Ok(bookStatuses);
        }

        public async Task<OperationalResult<BookStatusDto>> GetBookStatusById(int bookStatusId)
        {
            var bookStatus = _mapper.Map<BookStatusDto>(await _bookStatusRepository.GetBookStatusById(bookStatusId));

            if (bookStatus == null)
                return OperationalResult<BookStatusDto>.Error("No book status found.");

            return OperationalResult<BookStatusDto>.Ok(bookStatus);
        }
        public async Task<OperationalResult<bool>> CheckoutBookAsync(int bookId, int studentId)
        {
            var checkoutDate = DateTime.UtcNow;

            await using var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = "CheckoutBook";
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(new SqlParameter("@book_id", SqlDbType.Int) { Value = bookId });
            command.Parameters.Add(new SqlParameter("@student_id", SqlDbType.Int) { Value = studentId });
            command.Parameters.Add(new SqlParameter("@date_checkout", SqlDbType.DateTime) { Value = checkoutDate });

            var returnParam = new SqlParameter
            {
                Direction = ParameterDirection.ReturnValue,
                SqlDbType = SqlDbType.Int
            };
            command.Parameters.Add(returnParam);

            try
            {
                await command.ExecuteNonQueryAsync();

                var result = (int)returnParam.Value;

                if (result == 1)
                {
                    return OperationalResult<bool>.Ok(true);
                }
                else
                {
                    return OperationalResult<bool>.Error("No copies available.");
                }
            }
            catch (SqlException ex)
            {
                switch (ex.Number)
                {
                    case 50001:
                        return OperationalResult<bool>.Error("No available copies of this book.", ErrorCode.ValidationFailed);

                    case 50002:
                        return OperationalResult<bool>.Error("This student already has this book checked out.", ErrorCode.ValidationFailed);

                    default:
                        return OperationalResult<bool>.Error("Database error: " + ex.Message, ErrorCode.SaveFailed);
                }
            }
        }

        public async Task<OperationalResult<BookReturnedDto>> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState)
        {
            var bookStatus = await _bookStatusRepository.GetBookStatusById(id);
            if (bookStatus == null)
                return OperationalResult<BookReturnedDto>.Error("Book not found.");

            if (patchDoc == null)
                return OperationalResult<BookReturnedDto>.Error("Invalid patch document.");

            if (bookStatus.DateReturned.HasValue)
                return OperationalResult<BookReturnedDto>.Error("This book has already been returned.", ErrorCode.ValidationFailed);

            PatchHelper.TryApplyPatch<BookStatus>(patchDoc, bookStatus, modelState);

            if (!modelState.IsValid)
                return OperationalResult<BookReturnedDto>.Error("Patch validation failed.");

            var saved = await _bookStatusRepository.SaveAsync();

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

        public async Task<OperationalResult<BookReturnedDto>> ReturnBookByInt(int bookId)
        {
            var bookStatus = await _bookStatusRepository.GetBookStatusById(bookId);

            
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

            if (!await _bookStatusRepository.SaveAsync())
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
