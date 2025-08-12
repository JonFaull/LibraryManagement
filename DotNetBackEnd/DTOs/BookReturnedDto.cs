namespace LibraryMgmt.DTOs
{
    public class BookReturnedDto
    {
        public int? BookId { get; set; }
        public string? Title { get; set; }
        public string? StudentName { get; set; }
        public DateTime? DateReturned { get; set; }
    }
}
