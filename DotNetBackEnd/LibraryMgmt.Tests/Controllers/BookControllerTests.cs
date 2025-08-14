using LibraryMgmt.Controllers;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryMgmt.Tests.Controllers
{
    public class BookControllerTests
    {
        private readonly Mock<IBookService> _serviceMock;
        private readonly BookController _controller;

        public BookControllerTests()
        {
            _serviceMock = new Mock<IBookService>();
            _controller = new BookController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetBooks_ReturnsOk_WhenBooksExist()
        {
            var books = new List<BookDto> { new BookDto { BookId = 1, Title = "Test Book" } };
            var resultData = OperationalResult<ICollection<BookDto>>.Ok(books);

            _serviceMock.Setup(s => s.GetBooks()).ReturnsAsync(resultData);

            var result = await _controller.GetBooks();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<BookDto>>>(okResult.Value);
            Assert.True(returned.Success);
            Assert.Single(returned.Data);
        }

        [Fact]
        public async Task GetBooks_ReturnsNotFound_WhenServiceFails()
        {
            var resultData = OperationalResult<ICollection<BookDto>>.Error("No books found.");

            _serviceMock.Setup(s => s.GetBooks()).ReturnsAsync(resultData);

            var result = await _controller.GetBooks();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<BookDto>>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("No books found.", returned.Message);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsOk_WhenBookFound()
        {
            var book = new BookDto { BookId = 1, Title = "Test Book" };
            var resultData = OperationalResult<BookDto>.Ok(book);

            _serviceMock.Setup(s => s.GetBookById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatusById(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookDto>>(okResult.Value);
            Assert.True(returned.Success);
            Assert.Equal("Test Book", returned.Data.Title);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsNotFound_WhenBookMissing()
        {
            var resultData = OperationalResult<BookDto>.Error("Book not found.");

            _serviceMock.Setup(s => s.GetBookById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatusById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<Book>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("Book not found.", returned.Message);
        }

        [Fact]
        public async Task AddBook_ReturnsOk_WhenAddedSuccessfully()
        {
            var newBook = new BookDto { BookId = 2, Title = "New Book" };
            var resultData = OperationalResult<BookDto>.Ok(newBook);

            _serviceMock.Setup(s => s.AddBook(newBook)).ReturnsAsync(resultData);

            var result = await _controller.AddBook(newBook);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookDto>>(okResult.Value);
            Assert.True(returned.Success);
            Assert.Equal("New Book", returned.Data.Title);
        }

        [Fact]
        public async Task AddBook_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.AddBook(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookDto>>(badRequest.Value);
            Assert.False(returned.Success);
            Assert.Equal("Book data is required", returned.Message);
        }

        [Fact]
        public async Task AddBook_ReturnsNotFound_WhenServiceFails()
        {
            var newBook = new BookDto { BookId = 3, Title = "Fail Book" };
            var resultData = OperationalResult<BookDto>.Error("Failed to add book.");

            _serviceMock.Setup(s => s.AddBook(newBook)).ReturnsAsync(resultData);

            var result = await _controller.AddBook(newBook);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookDto>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("Failed to add book.", returned.Message);
        }
    }
}
