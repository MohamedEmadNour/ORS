namespace OMS.Service.ExceptionsHandeling
{
    public class ValidationErrorResponse : CustomException
    {
        public IEnumerable<string> Errors { get; set; }
        public ValidationErrorResponse()
            : base(400)
        {
        }
    }
}
