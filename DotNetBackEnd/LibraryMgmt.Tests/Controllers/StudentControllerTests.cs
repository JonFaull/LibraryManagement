using LibraryMgmt.Controllers;
using LibraryMgmt.DTOs;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace LibraryMgmt.Tests.Controllers
{
    public class StudentControllerTests
    {
        private readonly Mock<IStudentService> _serviceMock;
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _serviceMock = new Mock<IStudentService>();
            _controller = new StudentController(_serviceMock.Object);
        }

        [Fact]
        public async Task GetStudents_ReturnsOk_WhenDataExists()
        {
            var students = new List<StudentDto> { new StudentDto { StudentId = 1, FirstName = "Alice" } };
            var resultData = OperationalResult<ICollection<StudentDto>>.Ok(students);

            _serviceMock.Setup(s => s.GetStudents()).ReturnsAsync(resultData);

            var result = await _controller.GetStudents();

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<StudentDto>>>(ok.Value);
            Assert.True(returned.Success);
            Assert.Single(returned.Data);
        }

        [Fact]
        public async Task GetStudents_ReturnsNotFound_WhenServiceFails()
        {
            var resultData = OperationalResult<ICollection<StudentDto>>.Error("No students found.");

            _serviceMock.Setup(s => s.GetStudents()).ReturnsAsync(resultData);

            var result = await _controller.GetStudents();

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<ICollection<StudentDto>>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("No students found.", returned.Message);
        }

        [Fact]
        public async Task GetStudentById_ReturnsOk_WhenFound()
        {
            var student = new StudentDto { StudentId = 1, FirstName = "Bob" };
            var resultData = OperationalResult<StudentDto>.Ok(student);

            _serviceMock.Setup(s => s.GetStudentById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetStudentById(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<StudentDto>>(ok.Value);
            Assert.True(returned.Success);
            Assert.Equal("Bob", returned.Data.FirstName);
        }

        [Fact]
        public async Task GetStudentById_ReturnsNotFound_WhenMissing()
        {
            var resultData = OperationalResult<StudentDto>.Error("Student not found.");

            _serviceMock.Setup(s => s.GetStudentById(1)).ReturnsAsync(resultData);

            var result = await _controller.GetStudentById(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<StudentDto>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("Student not found.", returned.Message);
        }

        [Fact]
        public async Task AddStudent_ReturnsOk_WhenSuccessful()
        {
            var newStudent = new CreateStudentDto { FirstName = "Charlie", EmailAddress = "charlie@example.com" };
            var studentDto = new StudentDto { StudentId = 2, FirstName = "Charlie" };
            var resultData = OperationalResult<StudentDto>.Ok(studentDto);

            _serviceMock.Setup(s => s.AddStudent(newStudent)).ReturnsAsync(resultData);

            var result = await _controller.AddStudent(newStudent);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<StudentDto>>(ok.Value);
            Assert.True(returned.Success);
            Assert.Equal("Charlie", returned.Data.FirstName);
        }

        [Fact]
        public async Task AddStudent_ReturnsBadRequest_WhenNull()
        {
            var result = await _controller.AddStudent(null!);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<StudentDto>>(badRequest.Value);
            Assert.False(returned.Success);
            Assert.Equal("Student data is required", returned.Message);
        }

        [Fact]
        public async Task AddStudent_ReturnsNotFound_WhenServiceFails()
        {
            var newStudent = new CreateStudentDto { FirstName = "Dave", EmailAddress = "dave@example.com" };
            var resultData = OperationalResult<StudentDto>.Error("Failed to add student.");

            _serviceMock.Setup(s => s.AddStudent(newStudent)).ReturnsAsync(resultData);

            var result = await _controller.AddStudent(newStudent);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            var returned = Assert.IsAssignableFrom<OperationalResult<StudentDto>>(notFound.Value);
            Assert.False(returned.Success);
            Assert.Equal("Failed to add student.", returned.Message);
        }
    }
}
