using LibraryMgmt.DTOs;

namespace LibraryMgmt.Services.Interfaces
{
    public interface IStudentService
    {
        Task<OperationalResult<ICollection<StudentDto>>> GetStudents();

        Task<OperationalResult<StudentDto>> GetStudentById(int studentId);

        Task<OperationalResult<StudentDto>> AddStudent(CreateStudentDto student);

       
       /* OperationalResult<StudentDto> UpdateStudent(int studentId, StudentDto student);*/
    }
}
