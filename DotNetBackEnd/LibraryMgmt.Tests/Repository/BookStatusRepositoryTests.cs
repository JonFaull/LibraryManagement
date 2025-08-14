using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LibraryMgmt.Tests.Repository
{
    public class BookStatusRepositoryTests
    {
        // Helper method to create a fresh in-memory database context
        private DataContext GetInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        [Fact]
        public async Task GetBookStatuses_ReturnsAllStatuses()
        {
            // Arrange
            using var context = GetInMemoryContext();
            context.BookStatuses.AddRange(
                new BookStatus { BookStatusId = 1 },
                new BookStatus { BookStatusId = 2 }
            );
            await context.SaveChangesAsync();

            var repo = new BookStatusRepository(context);

            // Act
            var result = await repo.GetBookStatuses();

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetBookStatusById_ReturnsCorrectStatusWithIncludes()
        {
            // Arrange
            using var context = GetInMemoryContext();
            var book = new Book { BookId = 1, Title = "Test Book" };
            var student = new Student { StudentId = 1, FirstName = "John", LastName = "Murphy", CourseId = 1, EmailAddress = "john.murphy@gmail.com" };
            var status = new BookStatus
            {
                BookStatusId = 1,
                BookId = book.BookId,
                StudentId = student.StudentId,
                Book = book,
                Student = student,
                DateCheckout = DateTime.Today
            };

            context.Books.Add(book);
            context.Students.Add(student);
            context.BookStatuses.Add(status);
            await context.SaveChangesAsync();

            var repo = new BookStatusRepository(context);

            // Act
            var result = await repo.GetBookStatusById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Book.Title);
            Assert.Equal("John", result.Student.FirstName);
            Assert.Equal("Murphy", result.Student.LastName);
        }

        [Fact]
        public async Task BookStatusExists_ReturnsTrue_WhenExists()
        {
            using var context = GetInMemoryContext();
            context.BookStatuses.Add(new BookStatus { BookStatusId = 1 });
            await context.SaveChangesAsync();

            var repo = new BookStatusRepository(context);

            var exists = await repo.BookStatusExists(1);

            Assert.True(exists);
        }

        [Fact]
        public async Task ReturnBook_SetsDateReturnedAndSaves()
        {
            using var context = GetInMemoryContext();
            var status = new BookStatus
            {
                BookStatusId = 1,
                DateCheckout = DateTime.Today,
                DateReturned = null
            };
            context.BookStatuses.Add(status);
            await context.SaveChangesAsync();

            var repo = new BookStatusRepository(context);

            var result = await repo.ReturnBook(status);

            Assert.True(result);
            Assert.NotNull(status.DateReturned);
            Assert.True((DateTime.Now - status.DateReturned.Value).TotalSeconds < 5);
        }
    }
}
