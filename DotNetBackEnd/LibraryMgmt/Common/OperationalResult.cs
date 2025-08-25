namespace LibraryMgmt.Common
{
    public enum ErrorCode
    {
        NotFound,
        ValidationFailed,
        SaveFailed,
        Unauthorized,
        Unknown,
        InternalServerError
    }

    public class OperationalResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public ErrorCode? Code { get; set; }

        public OperationalResult() { }

        private OperationalResult(bool success, string message, T data = default, ErrorCode? code = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Code = code;
        }

        public static OperationalResult<T> Ok(T data = default)
            => new OperationalResult<T>(true, "Operation completed successfully.", data);




        public static OperationalResult<T> Error(string message, ErrorCode code = ErrorCode.Unknown)
            => new OperationalResult<T>(false, message, default, code);



    }

}
