namespace LibraryMgmt.Common
{
    public interface IGlobalExceptionHandler
    {
        IResult Handle(Exception exception);
    }
}
