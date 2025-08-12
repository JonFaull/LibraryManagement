using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryMgmt.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Course id is required.")]
        public int? CourseId { get; set; }

        [Required]
        public string EmailAddress { get; set; }
    }
}
