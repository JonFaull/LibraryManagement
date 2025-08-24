using LibraryMgmt.Models;
using AutoMapper;
using LibraryMgmt.DTOs;

namespace LibraryMgmt.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();
            CreateMap<BookStatus, BookStatusDto>();
            CreateMap<BookStatusDto, BookStatus>();
            CreateMap<BookStatus, BookReturnedDto>();
            CreateMap<StudentDto, Student>();
            CreateMap<Student, StudentDto>();
            CreateMap<CreateStudentDto, Student>();
            CreateMap<AddBookDto, Book>();
        }
    }
}
