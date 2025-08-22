using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryMgmt.DTOs
{
    public class AddBookDto
    {
        [Required(ErrorMessage = "Title is required.")]
        public String Title { get; set; }
        [Required(ErrorMessage = "Author is required.")]
        public String Author { get; set; }
        [Required(ErrorMessage = "ISBN is required.")]
        public String Isbn { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Number of copies must be 0 or positive.")]

        [DefaultValue(0)]
        public int NoCopies { get; set; } = 0;
    }
}
