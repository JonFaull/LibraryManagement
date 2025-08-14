using AutoMapper;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace LibraryMgmt.Tests.Services
{
    public class BookStatusServiceTests
    {
        private readonly Mock<IBookStatusRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly BookStatusService _service;

        public BookStatusServiceTests()
        {
            _repoMock = new Mock<IBookStatusRepository>();
            _mapperMock = new Mock<IMapper>();
            var context = new Mock<Data.DataContext>().Object; // Not used directly in tests
            _service = new BookStatusService(_repoMock.Object, context, _mapperMock.Object);
        }

        [Fact]
        public async Task GetBookStatuses_ReturnsMappedDtos_WhenDataExists()
        {
            var statuses = new List<BookStatus> { new BookStatus { BookStatusId = 1 } };
            var dtos = new List<BookStatusDto> { new BookStatusDto { BookStatusId = 1 } };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(statuses);
            _mapperMock.Setup(m => m.Map<ICollection<BookStatusDto>>(statuses)).Returns(dtos);

            var result = await _service.GetBookStatuses();

            Assert.True(result.Success);
            Assert.Single(result.Data);
        }

        [Fact]
        public async Task GetBookStatuses_ReturnsError_WhenNoData()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<BookStatus>());
            _mapperMock.Setup(m => m.Map<ICollection<BookStatusDto>>(It.IsAny<ICollection<BookStatus>>()))
                .Returns(new List<BookStatusDto>());

            var result = await _service.GetBookStatuses();

            Assert.False(result.Success);
            Assert.Equal("No book statuses found.", result.Message);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsMappedDto_WhenFound()
        {
            var status = new BookStatus { BookStatusId = 1 };
            var dto = new BookStatusDto { BookStatusId = 1 };

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);
            _mapperMock.Setup(m => m.Map<BookStatusDto>(status)).Returns(dto);

            var result = await _service.GetBookStatusById(1);

            Assert.True(result.Success);
            Assert.Equal(1, result.Data.BookStatusId);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsError_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync((BookStatus)null!);
            _mapperMock.Setup(m => m.Map<BookStatusDto>(null!)).Returns((BookStatusDto)null!);

            var result = await _service.GetBookStatusById(1);

            Assert.False(result.Success);
            Assert.Equal("No book status found.", result.Message);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsError_WhenAlreadyReturned()
        {
            var status = new BookStatus { BookId = 1, DateReturned = DateTime.UtcNow };

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);

            var result = await _service.ReturnBookByInt(1);

            Assert.False(result.Success);
            Assert.Equal("This book has already been returned.", result.Message);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsSuccess_WhenValid()
        {
            var status = new BookStatus
            {
                BookId = 1,
                Book = new Book { Title = "Test Book" },
                Student = new Student { FirstName = "Alice", LastName = "Smith" },
                DateReturned = null
            };

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);
            _repoMock.Setup(r => r.SaveAsync()).ReturnsAsync(true);

            var result = await _service.ReturnBookByInt(1);

            Assert.True(result.Success);
            Assert.Equal("Test Book", result.Data.Title);
            Assert.Equal("Alice Smith", result.Data.StudentName);
        }

        [Fact]
        public async Task ReturnBook_ReturnsError_WhenPatchInvalid()
        {
            var status = new BookStatus { BookStatusId = 1, DateReturned = null };
            var patchDoc = new JsonPatchDocument<BookStatus>();
            var modelState = new ModelStateDictionary();

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);
            _repoMock.Setup(r => r.SaveAsync()).ReturnsAsync(true);

            modelState.AddModelError("DateReturned", "Required");

            var result = await _service.ReturnBook(1, patchDoc, modelState);

            Assert.False(result.Success);
            Assert.Equal("Patch validation failed.", result.Message);
        }

        [Fact]
        public async Task ReturnBook_ReturnsError_WhenAlreadyReturned()
        {
            var status = new BookStatus { BookStatusId = 1, DateReturned = DateTime.UtcNow };
            var patchDoc = new JsonPatchDocument<BookStatus>();
            var modelState = new ModelStateDictionary();

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);

            var result = await _service.ReturnBook(1, patchDoc, modelState);

            Assert.False(result.Success);
            Assert.Equal("This book has already been returned.", result.Message);
        }

        [Fact]
        public async Task ReturnBook_ReturnsError_WhenPatchDocIsNull()
        {
            var status = new BookStatus { BookStatusId = 1, DateReturned = null };
            var modelState = new ModelStateDictionary();

            _repoMock.Setup(r => r.GetBookStatusById(1)).ReturnsAsync(status);

            var result = await _service.ReturnBook(1, null!, modelState);

            Assert.False(result.Success);
            Assert.Equal("Invalid patch document.", result.Message);
        }
    }
}
