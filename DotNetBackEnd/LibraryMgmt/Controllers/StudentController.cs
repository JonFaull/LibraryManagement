using LibraryMgmt.DTOs;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace LibraryMgmt.Controllers
{
    [Authorize(Roles = "Admin,User")]
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet("GetStudents")]
        [ProducesResponseType(typeof(OperationalResult<ICollection<StudentDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationalResult<ICollection<StudentDto>>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OperationalResult<ICollection<StudentDto>>>> GetStudents()
        {
            var result = await _studentService.GetStudents();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<StudentDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("GetStudentsById{studentId:int}")]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OperationalResult<StudentDto>>> GetStudentById(int studentId)
        {
            var result = await _studentService.GetStudentById(studentId);

            if (!result.Success)
                return NotFound(OperationalResult<StudentDto>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddStudent")]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OperationalResult<StudentDto>>> AddStudent([FromBody] CreateStudentDto newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest(OperationalResult<StudentDto>.Error("Student data is required", ErrorCode.ValidationFailed));
            }

            var result = await _studentService.AddStudent(newStudent);

            if (!result.Success)
                return BadRequest(OperationalResult<StudentDto>.Error(result.Message, result.Code ?? ErrorCode.ValidationFailed));

            return CreatedAtAction(nameof(GetStudentById), new { studentId = result.Data?.StudentId }, result);
        }
    }
}
