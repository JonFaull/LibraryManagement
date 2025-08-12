using LibraryMgmt.Data;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;

namespace LibraryMgmt.Repository
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        private readonly DataContext _context;
        public StudentRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public Student GetStudentById(int studentId)
        {
            return _context.Students.FirstOrDefault(sd => sd.StudentId == studentId);
        }
    }
}
