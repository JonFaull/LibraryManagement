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
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryMgmt.Common;

namespace LibraryMgmt.Services
{
    public class BookStatusService : IBookStatusService
    {
        private readonly IBookStatusRepository _bookStatusRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BookStatusService> _logger;

        public BookStatusService(IBookStatusRepository bookStatusRepository, DataContext context, IMapper mapper, ILogger<BookStatusService> logger)
        {
            _bookStatusRepository = bookStatusRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationalResult<ICollection<BookStatusDto>>> GetBookStatuses()
        {
            _logger.LogInformation("Fetching all book statuses...");

            var bookStatuses = _mapper.Map<ICollection<BookStatusDto>>(await _bookStatusRepository.GetAllAsync());

            if (bookStatuses == null || bookStatuses.Count == 0)
            {
                _logger.LogWarning("No book statuses found.");
                return OperationalResult<ICollection<BookStatusDto>>.Error("No book statuses found.");
            }

            _logger.LogInformation("Retrieved {Count} book statuses.", bookStatuses.Count);
            return OperationalResult<ICollection<BookStatusDto>>.Ok(bookStatuses);
        }


        public async Task<OperationalResult<BookStatusDto>> GetBookStatusById(int bookStatusId)
        {
            _logger.LogInformation("Fetching book status with ID: {Id}", bookStatusId);

            var bookStatus = _mapper.Map<BookStatusDto>(await _bookStatusRepository.GetBookStatusById(bookStatusId));

            if (bookStatus == null)
            {
                _logger.LogWarning("No book status found for ID: {Id}", bookStatusId);
                return OperationalResult<BookStatusDto>.Error("No book status found.");
            }

            _logger.LogInformation("Book status retrieved for ID: {Id}", bookStatusId);
            return OperationalResult<BookStatusDto>.Ok(bookStatus);
        }

        public async Task<OperationalResult<bool>> CheckoutBookAsync(int bookId, int studentId)
        {
            _logger.LogInformation("Attempting to checkout book {BookId} for student {StudentId}", bookId, studentId);

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
                    _logger.LogInformation("Checkout successful for book {BookId} and student {StudentId}", bookId, studentId);
                    return OperationalResult<bool>.Ok(true);
                }

                _logger.LogWarning("Checkout failed: No copies available for book {BookId}", bookId);
                return OperationalResult<bool>.Error("No copies available.");
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL error during checkout for book {BookId} and student {StudentId}", bookId, studentId);

                return ex.Number switch
                {
                    50001 => OperationalResult<bool>.Error("No available copies of this book.", ErrorCode.ValidationFailed),
                    50002 => OperationalResult<bool>.Error("This student already has this book checked out.", ErrorCode.ValidationFailed),
                    _ => OperationalResult<bool>.Error("Database error: " + ex.Message, ErrorCode.SaveFailed)
                };
            }
        }


        public async Task<OperationalResult<BookReturnedDto>> ReturnBook(int id, JsonPatchDocument<BookStatus> patchDoc, ModelStateDictionary modelState)
        {
            _logger.LogInformation("Attempting to return book status with ID: {Id}", id);

            var bookStatus = await _bookStatusRepository.GetBookStatusById(id);
            if (bookStatus == null)
            {
                _logger.LogWarning("Book status not found for ID: {Id}", id);
                return OperationalResult<BookReturnedDto>.Error("Book not found.");
            }

            if (patchDoc == null)
            {
                _logger.LogWarning("Patch document is null for book status ID: {Id}", id);
                return OperationalResult<BookReturnedDto>.Error("Invalid patch document.");
            }

            if (bookStatus.DateReturned.HasValue)
            {
                _logger.LogWarning("Book status ID {Id} already marked as returned.", id);
                return OperationalResult<BookReturnedDto>.Error("This book has already been returned.", ErrorCode.ValidationFailed);
            }

            try
            {
                PatchHelper.TryApplyPatch<BookStatus>(patchDoc, bookStatus, modelState);
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying patch document for book status ID: {Id}", id);
                return OperationalResult<BookReturnedDto>.Error("Error applying patch document.");
            }


            if (!modelState.IsValid)
            {
                _logger.LogWarning("Patch validation failed for book status ID: {Id}", id);
                return OperationalResult<BookReturnedDto>.Error("Patch validation failed.");
            }

            var saved = await _bookStatusRepository.SaveAsync();

            var dto = new BookReturnedDto
            {
                BookId = bookStatus.BookId,
                Title = bookStatus.Book?.Title ?? "Unknown",
                StudentName = $"{bookStatus.Student?.FirstName ?? "Unknown"} {bookStatus.Student?.LastName ?? ""}".Trim(),
                DateReturned = bookStatus.DateReturned ?? DateTime.MinValue
            };

            if (!saved)
            {
                _logger.LogError("Failed to save return for book status ID: {Id}", id);
                return OperationalResult<BookReturnedDto>.Error("Failed to save changes.");
            }

            _logger.LogInformation("Book successfully returned for book status ID: {Id}", id);
            return OperationalResult<BookReturnedDto>.Ok(dto);
        }


        public async Task<OperationalResult<BookReturnedDto>> ReturnBookByInt(int bookId, string? userEmail)
        {
            _logger.LogInformation("Attempting to return book {BookId} for user {Email}", bookId, userEmail);

            var bookStatus = await _bookStatusRepository.GetBookStatusById(bookId);

            if (bookStatus == null)
            {
                _logger.LogWarning("No matching checkout found for book {BookId}", bookId);
                return OperationalResult<BookReturnedDto>.Error("No matching checkout found", ErrorCode.NotFound);
            }

            if (bookStatus.Student == null || bookStatus.Student.EmailAddress != userEmail)
            {
                _logger.LogWarning("Book {BookId} is not checked out by user {Email}", bookId, userEmail);
                return OperationalResult<BookReturnedDto>.Error("This book is not checked out by the current user.", ErrorCode.ValidationFailed);
            }

            if (bookStatus.DateReturned.HasValue)
            {
                _logger.LogWarning("Book {BookId} has already been returned.", bookId);
                return OperationalResult<BookReturnedDto>.Error("This book has already been returned.", ErrorCode.ValidationFailed);
            }

            bookStatus.DateReturned = DateTime.UtcNow;

            var dto = new BookReturnedDto
            {
                BookId = bookStatus.BookId,
                Title = bookStatus.Book?.Title ?? "Unknown",
                StudentName = $"{bookStatus.Student?.FirstName ?? "Unknown"} {bookStatus.Student?.LastName ?? ""}".Trim(),
                DateReturned = bookStatus.DateReturned ?? DateTime.MinValue
            };

            if (!await _bookStatusRepository.SaveAsync())
            {
                _logger.LogError("Failed to save return for book {BookId}", bookId);
                return OperationalResult<BookReturnedDto>.Error("Something went wrong returning the book.", ErrorCode.SaveFailed);
            }

            _logger.LogInformation("Book {BookId} successfully returned by user {Email}", bookId, userEmail);
            return OperationalResult<BookReturnedDto>.Ok(dto);
        }


        private int GetBookStatusBy(int studentId, int bookId)
        {
            _logger.LogDebug("Fetching book status for student {StudentId} and book {BookId}", studentId, bookId);

            var bookStatus = _context.BookStatuses
                .FirstOrDefault(bs => bs.StudentId == studentId && bs.BookId == bookId && bs.DateReturned == null);

            if (bookStatus == null)
            {
                _logger.LogWarning("No active book status found for student {StudentId} and book {BookId}", studentId, bookId);
                return -1;
            }

            _logger.LogInformation("Found book status ID {StatusId} for student {StudentId} and book {BookId}", bookStatus.BookStatusId, studentId, bookId);
            return bookStatus.BookStatusId;
        }

    }
}
