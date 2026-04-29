namespace TaskManagementMvc.Services
{
    public class ApiRequestException : Exception
    {
        public ApiRequestException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}
