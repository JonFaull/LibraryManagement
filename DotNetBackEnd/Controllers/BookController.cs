using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Models;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<BookDto>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<BookDto>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<BookDto>>))]


        public async Task<IActionResult?> GetBooks()
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<ICollection<BookDto>>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _bookService.GetBooks();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<BookDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("{bookId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookDto>))]
        public async Task<IActionResult> GetBookStatusById(int bookId)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<Book>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _bookService.GetBookById(bookId);

            if (!result.Success)
                return NotFound(OperationalResult<Book>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookDto newBook)
        {
            if(newBook == null)
            {
                return BadRequest(OperationalResult<BookDto>.Error("Book data is required", ErrorCode.ValidationFailed));
            }

            var result = await _bookService.AddBook(newBook);

            if (!result.Success)
                return NotFound(OperationalResult<BookDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }
    }
}
