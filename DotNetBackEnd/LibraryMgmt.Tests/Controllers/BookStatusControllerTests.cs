using LibraryMgmt.Controllers;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;

namespace LibraryMgmt.Tests.Controllers
{
    public class BookStatusControllerTests
    {
        private readonly Mock<IBookStatusService> _serviceMock;
        private readonly BookStatusController _controller;

        public BookStatusControllerTests()
        {
            _serviceMock = new Mock<IBookStatusService>();
            _controller = new BookStatusController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetBookStatuses_ReturnsOk_WhenDataExists()
        {
            var dtos = new List<BookStatusDto> { new BookStatusDto { BookStatusId = 1 } };
            var resultData = OperationalResult<ICollection<BookStatusDto>>.Ok(dtos);

            _serviceMock.Setup(s => s.GetBookStatuses()).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatuses();

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<BookStatusDto>>>(ok.Value);
            Assert.True(returned.Success);
        }

        [Fact]
        public async Task GetBookStatuses_ReturnsNotFound_WhenServiceFails()
        {
            var resultData = OperationalResult<ICollection<BookStatusDto>>.Error("No book statuses found.");

            _serviceMock.Setup(s => s.GetBookStatuses()).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatuses();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<BookStatus>>>(notFound.Value);
            Assert.False(returned.Success);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsOk_WhenFound()
        {
            var dto = new BookStatusDto { BookStatusId = 1 };
            var resultData = OperationalResult<BookStatusDto>.Ok(dto);

            _serviceMock.Setup(s => s.GetBookStatusById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatusById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookStatusDto>>(ok.Value);
            Assert.True(returned.Success);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsNotFound_WhenMissing()
        {
            var resultData = OperationalResult<BookStatusDto>.Error("Not found");

            _serviceMock.Setup(s => s.GetBookStatusById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetBookStatusById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookStatus>>(notFound.Value);
            Assert.False(returned.Success);
        }

        [Fact]
        public async Task CheckoutBook_ReturnsOk_WhenSuccessful()
        {
            var resultData = OperationalResult<bool>.Ok(true);

            _serviceMock.Setup(s => s.CheckoutBookAsync(1, 1)).ReturnsAsync(resultData);

            var result = await _controller.CheckoutBook(1, 1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(ok.Value);
            Assert.True(returned.Success);
            Assert.Equal("Checked out successfully", returned.Data);
        }

        [Fact]
        public async Task CheckoutBook_ReturnsBadRequest_WhenFailed()
        {
            var resultData = OperationalResult<bool>.Error("No copies available.");

            _serviceMock.Setup(s => s.CheckoutBookAsync(1, 1)).ReturnsAsync(resultData);

            var result = await _controller.CheckoutBook(1, 1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(badRequest.Value);
            Assert.False(returned.Success);
            Assert.Equal("No copies available.", returned.Message);
        }

        [Fact]
        public async Task ReturnBook_ReturnsOk_WhenSuccessful()
        {
            var patchDoc = new JsonPatchDocument<BookStatus>();
            var dto = new BookReturnedDto { BookId = 1, Title = "Test Book" };
            var resultData = OperationalResult<BookReturnedDto>.Ok(dto);

            _serviceMock.Setup(s => s.ReturnBook(1, patchDoc, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(resultData);

            var result = await _controller.ReturnBook(1, patchDoc);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookReturnedDto>>(ok.Value);
            Assert.True(returned.Success);
        }

        [Fact]
        public async Task ReturnBook_ReturnsBadRequest_WhenValidationFails()
        {
            var patchDoc = new JsonPatchDocument<BookStatus>();
            var resultData = OperationalResult<BookReturnedDto>.Error("Patch validation failed.");

            _serviceMock.Setup(s => s.ReturnBook(1, patchDoc, It.IsAny<ModelStateDictionary>()))
                .ReturnsAsync(resultData);

            var result = await _controller.ReturnBook(1, patchDoc);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(badRequest.Value);
            Assert.False(returned.Success);
            Assert.Equal("Validation failed", returned.Message);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsOk_WhenSuccessful()
        {
            var dto = new BookReturnedDto { BookId = 1, Title = "Returned Book" };
            var resultData = OperationalResult<BookReturnedDto>.Ok(dto);

            _serviceMock.Setup(s => s.ReturnBookByInt(1)).ReturnsAsync(resultData);

            var result = await _controller.ReturnBookByInt(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<BookReturnedDto>>(ok.Value);
            Assert.True(returned.Success);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsNotFound_WhenNotFound()
        {
            var resultData = OperationalResult<BookReturnedDto>.Error("Not found", ErrorCode.NotFound);

            _serviceMock.Setup(s => s.ReturnBookByInt(1)).ReturnsAsync(resultData);

            var result = await _controller.ReturnBookByInt(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("Not found", returned.Message);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsBadRequest_WhenValidationFails()
        {
            var resultData = OperationalResult<BookReturnedDto>.Error("Already returned", ErrorCode.ValidationFailed);

            _serviceMock.Setup(s => s.ReturnBookByInt(1)).ReturnsAsync(resultData);

            var result = await _controller.ReturnBookByInt(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(badRequest.Value);
            Assert.False(returned.Success);
            Assert.Equal("Already returned", returned.Message);
        }

        [Fact]
        public async Task ReturnBookByInt_ReturnsServerError_WhenSaveFails()
        {
            var resultData = OperationalResult<BookReturnedDto>.Error("Save failed", ErrorCode.SaveFailed);

            _serviceMock.Setup(s => s.ReturnBookByInt(1)).ReturnsAsync(resultData);

            var result = await _controller.ReturnBookByInt(1);

            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
            var returned = Assert.IsAssignableFrom<OperationalResult<string>>(serverError.Value);
            Assert.False(returned.Success);
            Assert.Equal("Save failed", returned.Message);
        }
    }
}
