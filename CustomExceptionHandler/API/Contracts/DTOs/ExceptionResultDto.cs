using System.Net.Mime;

namespace API.Contracts.DTOs
{
    public class ExceptionResultDto
    {
        public ExceptionResultDto(string contentType, int httpStatusCode, string message)
        {
            ContentType = contentType;
            HttpStatusCode = httpStatusCode;
            Message = message;
        }

        public string ContentType { get; set; }
        public int HttpStatusCode { get; set; }
        public string Message { get; set; }
    }
}
