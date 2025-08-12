using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IStudentService
    {
        OperationalResult<ICollection<StudentDto>> GetStudents();

        OperationalResult<StudentDto> GetStudentById(int studentId);

       /* OperationalResult<StudentDto> AddStudent(StudentDto student);

        OperationalResult<StudentDto> UpdateStudent(int studentId, StudentDto student);*/
    }
}
