using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibraryMgmt.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(DataContext context) : base(context)
        {
        }

        public async Task<Student> GetStudentById(int studentId)
        {
            return await _context.Students.FirstOrDefaultAsync(sd => sd.StudentId == studentId);
        }

        public async Task<bool> StudentExistsViaEmail(string email)
        {
            return await _context.Students.AnyAsync(s => s.EmailAddress == email);
        }

        public async Task<Student> AddStudent(Student student)
        {
            _context.Students.Add(student);
            var success = await SaveAsync();
            if (!success)
            {
                return null;
            }
            return student;
        }
    }
}
