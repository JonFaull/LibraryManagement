using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Tests.Repository
{
    public class BookRepositoryTests
    {
        [Fact]
        public async Task GetBookById_ReturnsCorrectBook()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

            using var context = new DataContext(options);
            context.Books.Add(new Book { BookId = 1, Title = "Test Book" });
            await context.SaveChangesAsync();

            var repo = new BookRepository(context);
            var result = await repo.GetBookById(1);

            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
        }

        [Fact]
        public async Task BookExists_ReturnsTrue_WhenBookExists()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new DataContext(options);
            context.Books.Add(new Book { BookId = 1, Title = "TestBook", Author = "Darcy Adams", Isbn = "adfsj", NoCopies = 2 });
            await context.SaveChangesAsync();

            var repo = new BookRepository(context);

            var exists = await repo.BookExists("adfsj");

            Assert.True(exists);
        }


        [Fact]
        public async Task UpdateNoBooks_IncrementsCopies_WhenBookExists()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new DataContext(options);
            var book = new Book { BookId = 1, Title = "TestBook", Author = "Darcy Adams", Isbn = "adfsj", NoCopies = 2 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var repo = new BookRepository(context);

            var result = await repo.UpdateNoBooks(1, 3);

            Assert.True(result);
            Assert.Equal(5, book.NoCopies);
        }
    }
}
