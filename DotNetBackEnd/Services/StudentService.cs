using AutoMapper;
using LibraryMgmt.Data;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


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

        public OperationalResult<ICollection<StudentDto>> GetStudents()
        {
            var students = _mapper.Map<List<StudentDto>>(await _studentRepository.GetAll());

            if (students == null || students.Count == 0)
                return OperationalResult<ICollection<StudentDto>>.Error("No books found.");

            return OperationalResult<ICollection<StudentDto>>.Ok(students);
        }

        public OperationalResult<StudentDto> GetStudentById(int studentId)
        {
            var student = _mapper.Map<StudentDto>(_studentRepository.GetStudentById(studentId));

            if (student == null)
                return OperationalResult<StudentDto>.Error("No student found with the given ID.");

            return OperationalResult<StudentDto>.Ok(student);
        }
    }
    
}
