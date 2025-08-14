using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryMgmt.Tests.Repository
{
    public class StudentRepositoryTests
    {
        private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        [Fact]
        public async Task GetStudentById_ReturnsCorrectStudent()
        {
            using var context = GetInMemoryContext();
            var student = new Student
            {
                StudentId = 1,
                FirstName = "Alice",
                LastName = "Johnson",
                EmailAddress = "alice@example.com"
            };
            context.Students.Add(student);
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);

            var result = await repo.GetStudentById(1);

            Assert.NotNull(result);
            Assert.Equal("Alice", result.FirstName);
            Assert.Equal("Johnson", result.LastName);
        }

        [Fact]
        public async Task StudentExistsViaEmail_ReturnsTrue_WhenEmailExists()
        {
            using var context = GetInMemoryContext();
            context.Students.Add(new Student
            {
                StudentId = 1,
                FirstName = "Bob",
                LastName = "Smith",
                EmailAddress = "bob@example.com"
            });
            await context.SaveChangesAsync();

            var repo = new StudentRepository(context);

            var exists = await repo.StudentExistsViaEmail("bob@example.com");

            Assert.True(exists);
        }

        [Fact]
        public async Task AddStudent_AddsStudentSuccessfully()
        {
            using var context = GetInMemoryContext();
            var student = new Student
            {
                FirstName = "Charlie",
                LastName = "Brown",
                EmailAddress = "charlie@example.com"
            };

            var repo = new StudentRepository(context);

            var result = await repo.AddStudent(student);

            Assert.NotNull(result);
            Assert.Equal("Charlie", result.FirstName);
            Assert.Equal("Brown", result.LastName);

            var savedStudent = await context.Students.FirstOrDefaultAsync(s => s.EmailAddress == "charlie@example.com");
            Assert.NotNull(savedStudent);
        }
    }
}
