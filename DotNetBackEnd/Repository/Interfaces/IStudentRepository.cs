using LibraryMgmt.DTOs;
using LibraryMgmt.Models;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Student GetStudentById(int studentId);

        /*Book GetBookByIsbn(string isbn);

        bool BookExists(string isbn);

        bool AddBook(Book book);

        bool UpdateNoBooks(int bookId, int noCopies);*/
    }
}
