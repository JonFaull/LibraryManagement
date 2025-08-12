namespace LibraryMgmt.DTOs
{
    public class BookStatusDto
    {
        public int BookStatusId { get; set; }

        public int BookId { get; set; }

        public int StudentId { get; set; }

        public DateTime DateCheckout { get; set; }

        public DateTime? DateReturned { get; set; }
    }
}
