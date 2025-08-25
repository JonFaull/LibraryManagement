namespace LibraryMgmt.Common
{
    public class GlobalExceptionHandler : IGlobalExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public IResult Handle(Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred.");

            var result = OperationalResult<object>.Error(
                "An unexpected error occurred.",
                ErrorCode.InternalServerError
            );

            return Results.Json(result, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
