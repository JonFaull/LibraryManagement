using System.ComponentModel.DataAnnotations;

namespace LibraryMgmt.DTOs
{
    public class CreateStudentDto
    {
        [Required(ErrorMessage = "First name is required.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "Course id is required.")]
        public int? CourseId { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string EmailAddress { get; set; }
    }
}
