using LibraryMgmt.DTOs;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryMgmt.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<StudentDto>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<StudentDto>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<StudentDto>>))]
        
        public async Task<IActionResult> GetStudents()
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<ICollection<StudentDto>>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _studentService.GetStudents();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<StudentDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{studentId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<StudentDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<StudentDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<StudentDto>))]
        public async Task<IActionResult> GetStudentById(int studentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<StudentDto>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = await _studentService.GetStudentById(studentId);

            if (!result.Success)
                return NotFound(OperationalResult<StudentDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest(OperationalResult<StudentDto>.Error("Student data is required", ErrorCode.ValidationFailed));
            }

            var result = await _studentService.AddStudent(newStudent);

            if (!result.Success)
                return NotFound(OperationalResult<StudentDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }
    }
}
