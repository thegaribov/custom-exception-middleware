using API.Contracts.DTOs;
using API.Helpers.CustomExceptionHandler.Abstracts;
using System.Net.Mime;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using API.Middlewares;
using System.Text;
using System.Diagnostics;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class GeneralExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GeneralExceptionHandler> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GeneralExceptionHandler(
            ILogger<GeneralExceptionHandler> logger,
            IWebHostEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public ExceptionResultDto Handle(Exception exception)
        {
            if (_hostingEnvironment.IsProduction())
            {
                string responseMessage = "Internal server error happened.";
                _logger.LogCritical(exception, responseMessage);

                return new ExceptionResultDto(
                    MediaTypeNames.Application.Json,
                    (int)HttpStatusCode.InternalServerError,
                    JsonSerializer.Serialize(responseMessage));
            }
            else
            {
                var responseMessage = GetFailedRequestMessage(_httpContextAccessor.HttpContext!, exception);
                _logger.LogCritical(responseMessage);

                return new ExceptionResultDto(
                    MediaTypeNames.Text.Plain,
                    (int)HttpStatusCode.InternalServerError,
                    responseMessage);
            }
        }

        private string GetFailedRequestMessage(HttpContext context, Exception exception)
        {
            StringBuilder messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("Failed Request");
            messageBuilder.AppendLine($"\tSchema: {context.Request?.Scheme}");
            messageBuilder.AppendLine($"\tHost: {context.Request?.Host}");
            messageBuilder.AppendLine($"\tUser: {context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"}");
            messageBuilder.AppendLine($"\tMethod: {context.Request?.Method}");
            messageBuilder.AppendLine($"\tPath: {context.Request?.Path}");
            messageBuilder.AppendLine($"\tQueryString: {context.Request?.QueryString}");
            messageBuilder.AppendLine($"\tErrorMessage: {exception.Message}");
            messageBuilder.AppendLine("\tStacktrace:");

            if (exception.StackTrace != null)
            {
                string[] stackTraceLines = exception.StackTrace.Split('\n');
                foreach (string line in stackTraceLines)
                {
                    messageBuilder.AppendLine(line);
                }
            }

            if (exception.InnerException != null)
            {
                var separator = new string('=', 150);
                messageBuilder.AppendLine(separator);
                messageBuilder.AppendLine($"\tInnerException's ErrorMessage: {exception.InnerException.Message}");
                messageBuilder.AppendLine("\tInnerException's Stacktrace:");

                if (exception.InnerException.StackTrace != null)
                {
                    string[] innerStackTraceLines = exception.InnerException.StackTrace.Split('\n');
                    foreach (string line in innerStackTraceLines)
                    {
                        messageBuilder.AppendLine(line);
                    }
                }
            }

            return messageBuilder.ToString();
        }
    }
}
