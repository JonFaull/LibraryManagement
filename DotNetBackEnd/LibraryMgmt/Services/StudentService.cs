using AutoMapper;
using LibraryMgmt.Data;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;


namespace LibraryMgmt.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, DataContext context, IMapper mapper)
        {
            _studentRepository = studentRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<OperationalResult<ICollection<StudentDto>>> GetStudents()
        {
            var students = _mapper.Map<List<StudentDto>>(await _studentRepository.GetAllAsync());

            if (students == null || students.Count == 0)
                return OperationalResult<ICollection<StudentDto>>.Error("No books found.");

            return OperationalResult<ICollection<StudentDto>>.Ok(students);
        }

        public async Task<OperationalResult<StudentDto>> GetStudentById(int studentId)
        {
            var student = _mapper.Map<StudentDto>(await _studentRepository.GetStudentById(studentId));

            if (student == null)
                return OperationalResult<StudentDto>.Error("No student found with the given ID.");

            return OperationalResult<StudentDto>.Ok(student);
        }

        public async Task<OperationalResult<StudentDto>> AddStudent(CreateStudentDto studentDto)
        {
            var studentExists = await _studentRepository.StudentExistsViaEmail(studentDto.EmailAddress);

            if (studentExists)
            {
                return OperationalResult<StudentDto>.Error("A student with this email already exists.", ErrorCode.ValidationFailed);
            }

            var addedStudent = await _studentRepository.AddStudent(_mapper.Map<Student>(studentDto));
            
            if(addedStudent!= null)
                return OperationalResult<StudentDto>.Ok(_mapper.Map<StudentDto>(addedStudent));
            else
                return OperationalResult<StudentDto>.Error("Failed to add new student.", ErrorCode.SaveFailed);
        }
    }
    
}
