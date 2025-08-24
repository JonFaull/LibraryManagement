using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Models;
using LibraryMgmt.DTOs;
using Microsoft.AspNetCore.Authorization;
using LibraryMgmt.Common;

namespace LibraryMgmt.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetBooks")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<BookDto>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<BookDto>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<BookDto>>))]

        public async Task<IActionResult> GetBooks()
        {
            var result = await _bookService.GetBooks();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<BookDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("GetBookById/{bookId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookDto>))]
        public async Task<IActionResult> GetBookStatusById(int bookId)
        {
            var result = await _bookService.GetBookById(bookId);

            if (!result.Success)
                return NotFound(OperationalResult<BookDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddBook")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookDto>))]
        public async Task<IActionResult> AddBook([FromBody] AddBookDto newBook)
        {
            if (newBook == null)
            {
                return BadRequest(OperationalResult<BookDto>.Error("Book data is required", ErrorCode.ValidationFailed));
            }

            var result = await _bookService.AddBook(newBook); // should return OperationalResult<BookDto>

            if (!result.Success)
            {
                return result.Code switch
                {
                    ErrorCode.NotFound => NotFound(OperationalResult<BookDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound)),
                    ErrorCode.ValidationFailed => BadRequest(OperationalResult<BookDto>.Error(result.Message, result.Code ?? ErrorCode.ValidationFailed)),
                    ErrorCode.SaveFailed => StatusCode(500, OperationalResult<BookDto>.Error(result.Message, result.Code ?? ErrorCode.SaveFailed)),
                    _ => StatusCode(500, OperationalResult<BookDto>.Error("Unexpected error", ErrorCode.Unknown))
                };
            }

            return Ok(result);
        }

    }
}
