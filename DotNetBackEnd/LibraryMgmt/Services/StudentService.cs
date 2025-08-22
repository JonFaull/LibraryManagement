using AutoMapper;
using LibraryMgmt.Data;
using LibraryMgmt.DTOs;
using LibraryMgmt.Models;
using LibraryMgmt.Repository.Interfaces;
using LibraryMgmt.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace LibraryMgmt.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IStudentRepository studentRepository,
            DataContext context,
            IMapper mapper,
            ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<OperationalResult<ICollection<StudentDto>>> GetStudents()
        {
            _logger.LogInformation("Fetching all students...");

            var students = _mapper.Map<List<StudentDto>>(await _studentRepository.GetAllAsync());

            if (students == null || students.Count == 0)
            {
                _logger.LogWarning("No students found in the database.");
                return OperationalResult<ICollection<StudentDto>>.Error("No students found.");
            }

            _logger.LogInformation("Retrieved {Count} students.", students.Count);
            return OperationalResult<ICollection<StudentDto>>.Ok(students);
        }

        public async Task<OperationalResult<StudentDto>> GetStudentById(int studentId)
        {
            _logger.LogInformation("Fetching student with ID: {StudentId}", studentId);

            var student = _mapper.Map<StudentDto>(await _studentRepository.GetStudentById(studentId));

            if (student == null)
            {
                _logger.LogWarning("No student found with ID: {StudentId}", studentId);
                return OperationalResult<StudentDto>.Error("No student found with the given ID.");
            }

            _logger.LogInformation("Student found: {Name}", $"{student.FirstName ?? "Unknown"} {student.LastName ?? ""}".Trim());
            return OperationalResult<StudentDto>.Ok(student);
        }

        public async Task<OperationalResult<StudentDto>> AddStudent(CreateStudentDto studentDto)
        {
            _logger.LogInformation("Attempting to add student with email: {Email}", studentDto.EmailAddress);

            var studentExists = await _studentRepository.StudentExistsViaEmail(studentDto.EmailAddress);

            if (studentExists)
            {
                _logger.LogWarning("Student with email {Email} already exists.", studentDto.EmailAddress);
                return OperationalResult<StudentDto>.Error("A student with this email already exists.", ErrorCode.ValidationFailed);
            }

            var addedStudent = await _studentRepository.AddStudent(_mapper.Map<Student>(studentDto));

            if (addedStudent != null)
            {
                _logger.LogInformation("Successfully added student: {Name}", addedStudent.FirstName + " " + addedStudent.LastName);
                return OperationalResult<StudentDto>.Ok(_mapper.Map<StudentDto>(addedStudent));
            }

            _logger.LogError("Failed to add new student with email: {Email}", studentDto.EmailAddress);
            return OperationalResult<StudentDto>.Error("Failed to add new student.", ErrorCode.SaveFailed);
        }
    }
}
