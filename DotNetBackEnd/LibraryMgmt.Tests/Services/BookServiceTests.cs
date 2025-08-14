using AutoMapper;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services;
using LibraryMgmt.Services.Interfaces;
using Moq;

namespace LibraryMgmt.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _bookRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly IBookService _bookService;

        public BookServiceTests()
        {
            _bookRepoMock = new Mock<IBookRepository>();
            _mapperMock = new Mock<IMapper>();
            _bookService = new BookService(_bookRepoMock.Object, null!, _mapperMock.Object);
        }

        [Fact]
        public async Task GetBooks_ReturnsBooks_WhenBooksExist()
        {
            var books = new List<Book> { new Book { BookId = 1, Title = "Test Book" } };
            var bookDtos = new List<BookDto> { new BookDto { BookId = 1, Title = "Test Book" } };

            _bookRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(books);
            _mapperMock.Setup(m => m.Map<List<BookDto>>(books)).Returns(bookDtos);

            var result = await _bookService.GetBooks();

            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal("Test Book", result.Data.First().Title);
        }

        [Fact]
        public async Task GetBooks_ReturnsError_WhenNoBooksExist()
        {
            _bookRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Book>());
            _mapperMock.Setup(m => m.Map<List<BookDto>>(It.IsAny<List<Book>>())).Returns(new List<BookDto>());

            var result = await _bookService.GetBooks();

            Assert.False(result.Success);
            Assert.Equal("No books found.", result.Message);
        }

        [Fact]
        public async Task GetBookById_ReturnsBook_WhenFound()
        {
            var book = new Book { BookId = 1, Title = "Test Book" };
            var bookDto = new BookDto { BookId = 1, Title = "Test Book" };

            _bookRepoMock.Setup(r => r.GetBookById(1)).ReturnsAsync(book);
            _mapperMock.Setup(m => m.Map<BookDto>(book)).Returns(bookDto);

            var result = await _bookService.GetBookById(1);

            Assert.True(result.Success);
            Assert.Equal("Test Book", result.Data.Title);
        }

        [Fact]
        public async Task GetBookById_ReturnsError_WhenNotFound()
        {
            _bookRepoMock.Setup(r => r.GetBookById(1)).ReturnsAsync((Book)null!);
            _mapperMock.Setup(m => m.Map<BookDto>(null!)).Returns((BookDto)null!);

            var result = await _bookService.GetBookById(1);

            Assert.False(result.Success);
            Assert.Equal("No book found with the given ID.", result.Message);
        }

        [Fact]
        public async Task AddBook_UpdatesExistingBook_WhenIsbnExists()
        {
            var existingBook = new Book { BookId = 1, Isbn = "123", NoCopies = 2 };
            var updatedBook = new Book { BookId = 1, Isbn = "123", NoCopies = 5 };
            var bookDto = new BookDto { BookId = 1, Isbn = "123", NoCopies = 3 };

            _bookRepoMock.Setup(r => r.GetBookByIsbn("123")).ReturnsAsync(existingBook);
            _bookRepoMock.Setup(r => r.UpdateNoBooks(1, 3)).ReturnsAsync(true);
            _bookRepoMock.Setup(r => r.GetBookByIsbn("123")).ReturnsAsync(updatedBook);
            _mapperMock.Setup(m => m.Map<BookDto>(updatedBook)).Returns(new BookDto { BookId = 1, Isbn = "123", NoCopies = 5 });

            var result = await _bookService.AddBook(bookDto);

            Assert.True(result.Success);
            Assert.Equal(5, result.Data.NoCopies);
        }

        [Fact]
        public async Task AddBook_AddsNewBook_WhenIsbnNotExists()
        {
            var bookDto = new BookDto { BookId = 2, Isbn = "456", NoCopies = 1 };
            var newBook = new Book { BookId = 2, Isbn = "456", NoCopies = 1 };

            _bookRepoMock.Setup(r => r.GetBookByIsbn("456")).ReturnsAsync((Book)null!);
            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Returns(newBook);
            _bookRepoMock.Setup(r => r.AddBook(newBook)).ReturnsAsync(newBook);
            _mapperMock.Setup(m => m.Map<BookDto>(newBook)).Returns(bookDto);

            var result = await _bookService.AddBook(bookDto);

            Assert.True(result.Success);
            Assert.Equal("456", result.Data.Isbn);
        }

        [Fact]
        public async Task AddBook_ReturnsError_WhenUpdateFails()
        {
            var existingBook = new Book { BookId = 1, Isbn = "123", NoCopies = 2 };
            var bookDto = new BookDto { BookId = 1, Isbn = "123", NoCopies = 3 };

            _bookRepoMock.Setup(r => r.GetBookByIsbn("123")).ReturnsAsync(existingBook);
            _bookRepoMock.Setup(r => r.UpdateNoBooks(1, 3)).ReturnsAsync(false);

            var result = await _bookService.AddBook(bookDto);

            Assert.False(result.Success);
            Assert.Equal("Failed to update existing book.", result.Message);
        }

        [Fact]
        public async Task AddBook_ReturnsError_WhenAddFails()
        {
            var bookDto = new BookDto { BookId = 2, Isbn = "789", NoCopies = 1 };
            var book = new Book { BookId = 2, Isbn = "789", NoCopies = 1 };

            _bookRepoMock.Setup(r => r.GetBookByIsbn("789")).ReturnsAsync((Book)null!);
            _mapperMock.Setup(m => m.Map<Book>(bookDto)).Returns(book);
            _bookRepoMock.Setup(r => r.AddBook(book)).ReturnsAsync((Book)null!);

            var result = await _bookService.AddBook(bookDto);
 
            Assert.False(result.Success);
            Assert.Equal("Failed to add new book.", result.Message);
        }
    }
}
