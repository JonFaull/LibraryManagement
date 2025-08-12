using LibraryMgmt.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using LibraryMgmt.Services.Interfaces;
using LibraryMgmt.Models;
using LibraryMgmt.Services;

namespace LibraryMgmt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(OperationalResult<ICollection<StudentDto>>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<ICollection<StudentDto>>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<ICollection<StudentDto>>))]


        public IActionResult GetStudents()
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<ICollection<StudentDto>>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = _studentService.GetStudents();

            if (!result.Success)
                return NotFound(OperationalResult<ICollection<StudentDto>>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }

        [HttpGet("{studentId:int}")]
        [ProducesResponseType(200, Type = typeof(OperationalResult<StudentDto>))]
        [ProducesResponseType(400, Type = typeof(OperationalResult<BookDto>))]
        [ProducesResponseType(404, Type = typeof(OperationalResult<BookDto>))]
        public IActionResult GetStudentById(int studentId)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationalResult<Student>.Error("Invalid model state.", ErrorCode.ValidationFailed));

            var result = _studentService.GetStudentById(studentId);

            if (!result.Success)
                return NotFound(OperationalResult<Student>.Error(result.Message, result.Code ?? ErrorCode.NotFound));

            return Ok(result);
        }
    }
}
