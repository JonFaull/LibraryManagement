using LibraryMgmt.DTOs;
using LibraryMgmt.Models;

namespace LibraryMgmt.Repository.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student> GetStudentById(int studentId);

        Task<bool> StudentExistsViaEmail(string email);
        
        Task<Student> AddStudent(Student student);
        /*Book GetBookByIsbn(string isbn);

        bool BookExists(string isbn);

        bool AddBook(Book book);

        bool UpdateNoBooks(int bookId, int noCopies);*/
    }
}
