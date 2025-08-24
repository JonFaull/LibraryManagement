using LibraryMgmt.DTOs;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LibraryMgmt.Common;

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
        public async Task<IActionResult> GetStudents()

        {
            var result = await _studentService.GetStudents();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<StudentDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("GetStudent/{studentId}")]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudentById(int studentId)
        {
            var result = await _studentService.GetStudentById(studentId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddStudent")]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(OperationalResult<StudentDto>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddStudent([FromBody] CreateStudentDto newStudent)
        {
            if (newStudent == null)
            {
                return BadRequest(OperationalResult<StudentDto>.Error("Student data is required", ErrorCode.ValidationFailed));
            }

            var result = await _studentService.AddStudent(newStudent);

            if (!result.Success)
            {
                return result.Code switch
                {
                    ErrorCode.NotFound => NotFound(OperationalResult<StudentDto>.Error(result.Message, ErrorCode.NotFound)),
                    ErrorCode.ValidationFailed => BadRequest(OperationalResult<StudentDto>.Error(result.Message, ErrorCode.ValidationFailed)),
                    ErrorCode.SaveFailed => StatusCode(500, OperationalResult<StudentDto>.Error(result.Message, ErrorCode.SaveFailed)),
                    _ => StatusCode(500, OperationalResult<StudentDto>.Error("Unexpected error", ErrorCode.Unknown))
                };
            }

            return CreatedAtAction(
                nameof(GetStudentById),
                new { studentId = result.Data?.StudentId },
                result
            );
        }



    }
}
