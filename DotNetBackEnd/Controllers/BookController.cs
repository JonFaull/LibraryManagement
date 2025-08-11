using LibraryMgmt.Data;
using LibraryMgmt.Repository;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Models;
using LibraryMgmt.Services;

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
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<Book>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<Book>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<Book>>))]

        public IActionResult GetBooks()
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<ICollection<Book>>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = _bookService.GetBooks();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<Book>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("{bookId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<Book>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<Book>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<Book>))]
        public IActionResult GetBookStatusById(int bookId)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<Book>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = _bookService.GetBookById(bookId);

            if (!result.Success)
                return NotFound(OperationalResult<Book>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpPost]
        public IActionResult AddBook([FromBody] Book newBook)
        {
            if(newBook == null)
            {
                return BadRequest(OperationalResult<Book>.Error("Book data is required", ErrorCode.ValidationFailed));
            }

            var result = _bookService.AddBook(newBook);

            if (!result.Success)
                return NotFound(OperationalResult<Book>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }
    }
}
