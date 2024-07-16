namespace OMS.Service.ExceptionsHandeling
{
    public class CustomException : ApiResponse
    {
        public string? Details { get; set; }
        public CustomException(int statusCode, string? message = null, string? details = null) : base(statusCode, message)
        {
            Details = details;
        }
    }
}
