namespace LibraryMgmt
{
    public enum ErrorCode
    {
        NotFound,
        ValidationFailed,
        SaveFailed,
        Unauthorized,
        Unknown
    }

    public class OperationalResult<T>
    {
        public bool Success { get; }
        public string Message { get; }
        public T Data { get; }
        public ErrorCode? Code { get; }

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
