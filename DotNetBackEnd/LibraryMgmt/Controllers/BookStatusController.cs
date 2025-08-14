using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.JsonPatch;
using Swashbuckle.AspNetCore.Filters;
using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookStatusController : ControllerBase
    {
        private readonly IBookStatusService _bookStatusService;
        public BookStatusController(IBookStatusService bookStatusService)
        {
            _bookStatusService = bookStatusService;
        }

        [HttpGet]
       
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<BookStatusDto>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<BookStatusDto>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<BookStatusDto>>))]
        public async Task<IActionResult> GetBookStatuses()
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<ICollection<BookStatus>>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _bookStatusService.GetBookStatuses();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<BookStatus>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("{bookStatusId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookStatusDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookStatusDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookStatusDto>))]
        public async Task<IActionResult> GetBookStatusById(int bookStatusId)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<BookStatus>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _bookStatusService.GetBookStatusById(bookStatusId);

            if (!result.Success)
                return NotFound(OperationalResult<BookStatus>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(OperationalResult<string>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<string>))]
        [ProducesResponseType(500, Type = typeof(OperationalResult<string>))]
        public async Task<IActionResult> CheckoutBook(int bookId, int studentId)
        {
            try
            {
                var result = await _bookStatusService.CheckoutBookAsync(bookId, studentId);
                if (result.Success)
                {
                    return Ok(OperationalResult<string>.Ok("Checked out successfully"));
                }
                else
                {
                    return BadRequest(OperationalResult<string>.Error(result.Message, ErrorCode.ValidationFailed));
                }
            }
            catch (SqlException ex) when (ex.Number == 2627 || ex.Number == 2601)
            {
                return BadRequest(OperationalResult<string>.Error("This book is already checked out by this student.", ErrorCode.ValidationFailed));
            }
            catch (SqlException ex) when (ex.Number == 50001)
            {
                return BadRequest(OperationalResult<string>.Error(ex.Message, ErrorCode.ValidationFailed));
            }
            catch (SqlException ex)
            {
                return StatusCode(500, OperationalResult<string>.Error("Database error: " + ex.Message, ErrorCode.SaveFailed));
            }

        }

        [HttpPatch("{id}")]
        [Consumes("application/json-patch+json")]
        [SwaggerRequestExample(typeof(JsonPatchDocument<BookStatus>), typeof(JsonPatchExampleFilter))]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(500, Type = typeof(OperationalResult<BookReturnedDto>))]


        public async Task<IActionResult> ReturnBook(int id, [FromBody] JsonPatchDocument<BookStatus> patchDoc)
        {
            var result = await _bookStatusService.ReturnBook(id, patchDoc, ModelState);

            if (!result.Success)
            {
                if (result.Message.Contains("validation", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(OperationalResult<string>.Error("Validation failed", ErrorCode.ValidationFailed));

                return BadRequest(OperationalResult<string>.Error(result.Message));
            }

            return Ok(result);
        }

        [HttpPatch("{bookId}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookReturnedDto>))]
        [ProducesResponseType(500, Type = typeof(OperationalResult<BookReturnedDto>))]
        public async Task<IActionResult> ReturnBookByInt(int bookId)
        {
            var result = await _bookStatusService.ReturnBookByInt(bookId);

            if (!result.Success)
            {
                return result.Code switch
                {
                    ErrorCode.NotFound => NotFound(OperationalResult<string>.Error(result.Message, ErrorCode.NotFound)),
                    ErrorCode.ValidationFailed => BadRequest(OperationalResult<string>.Error(result.Message, ErrorCode.ValidationFailed)),
                    ErrorCode.SaveFailed => StatusCode(500, OperationalResult<string>.Error(result.Message, ErrorCode.SaveFailed)),
                    _ => StatusCode(500, OperationalResult<string>.Error("An unexpected error occurred.", ErrorCode.Unknown))
                };
            }

            return Ok(result);
        }
    }
}
