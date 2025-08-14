using AutoMapper;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services;
using Moq;

namespace LibraryMgmt.Tests.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<IStudentRepository> _repoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StudentService _service;

        public StudentServiceTests()
        {
            _repoMock = new Mock<IStudentRepository>();
            _mapperMock = new Mock<IMapper>();
            var context = new Mock<LibraryMgmt.Data.DataContext>().Object; 
            _service = new StudentService(_repoMock.Object, context, _mapperMock.Object);
        }

        [Fact]
        public async Task GetStudents_ReturnsMappedDtos_WhenDataExists()
        {
            var students = new List<Student> { new Student { StudentId = 1, FirstName = "Alice" } };
            var dtos = new List<StudentDto> { new StudentDto { StudentId = 1, FirstName = "Alice" } };

            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(students);
            _mapperMock.Setup(m => m.Map<List<StudentDto>>(students)).Returns(dtos);

            var result = await _service.GetStudents();

            Assert.True(result.Success);
            Assert.Single(result.Data);
            Assert.Equal("Alice", result.Data.First().FirstName);
        }

        [Fact]
        public async Task GetStudents_ReturnsError_WhenNoData()
        {
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Student>());
            _mapperMock.Setup(m => m.Map<List<StudentDto>>(It.IsAny<List<Student>>()))
                .Returns(new List<StudentDto>());

            var result = await _service.GetStudents();

            Assert.False(result.Success);
            Assert.Equal("No books found.", result.Message);
        }

        [Fact]
        public async Task GetStudentById_ReturnsMappedDto_WhenFound()
        {
            var student = new Student { StudentId = 1, FirstName = "Bob" };
            var dto = new StudentDto { StudentId = 1, FirstName = "Bob" };

            _repoMock.Setup(r => r.GetStudentById(1)).ReturnsAsync(student);
            _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(dto);

            var result = await _service.GetStudentById(1);

            Assert.True(result.Success);
            Assert.Equal("Bob", result.Data.FirstName);
        }

        [Fact]
        public async Task GetStudentById_ReturnsError_WhenNotFound()
        {
            _repoMock.Setup(r => r.GetStudentById(1)).ReturnsAsync((Student)null!);
            _mapperMock.Setup(m => m.Map<StudentDto>(null!)).Returns((StudentDto)null!);

            var result = await _service.GetStudentById(1);

            Assert.False(result.Success);
            Assert.Equal("No student found with the given ID.", result.Message);
        }

        [Fact]
        public async Task AddStudent_ReturnsError_WhenEmailExists()
        {
            var dto = new CreateStudentDto { EmailAddress = "test@example.com" };

            _repoMock.Setup(r => r.StudentExistsViaEmail("test@example.com")).ReturnsAsync(true);

            var result = await _service.AddStudent(dto);

            Assert.False(result.Success);
            Assert.Equal("A student with this email already exists.", result.Message);
        }

        [Fact]
        public async Task AddStudent_ReturnsSuccess_WhenAdded()
        {
            var dto = new CreateStudentDto { EmailAddress = "new@example.com", FirstName = "Charlie", LastName = "Brown" };
            var student = new Student { StudentId = 2, EmailAddress = "new@example.com", FirstName = "Charlie", LastName = "Brown" };
            var mappedDto = new StudentDto { StudentId = 2, FirstName = "Charlie", LastName = "Brown" };

            _repoMock.Setup(r => r.StudentExistsViaEmail("new@example.com")).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Student>(dto)).Returns(student);
            _repoMock.Setup(r => r.AddStudent(student)).ReturnsAsync(student);
            _mapperMock.Setup(m => m.Map<StudentDto>(student)).Returns(mappedDto);

            var result = await _service.AddStudent(dto);

            Assert.True(result.Success);
            Assert.Equal("Charlie", result.Data.FirstName);
        }

        [Fact]
        public async Task AddStudent_ReturnsError_WhenAddFails()
        {
            var dto = new CreateStudentDto { EmailAddress = "fail@example.com" };
            var student = new Student { EmailAddress = "fail@example.com" };

            _repoMock.Setup(r => r.StudentExistsViaEmail("fail@example.com")).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Student>(dto)).Returns(student);
            _repoMock.Setup(r => r.AddStudent(student)).ReturnsAsync((Student)null!);

            var result = await _service.AddStudent(dto);

            Assert.False(result.Success);
            Assert.Equal("Failed to add new student.", result.Message);
        }
    }
}
