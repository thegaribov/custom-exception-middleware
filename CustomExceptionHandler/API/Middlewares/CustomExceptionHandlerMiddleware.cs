using API.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next, 
            ILoggerFactory loggerFactory,
            IWebHostEnvironment hostingEnvironment)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CustomExceptionHandlerMiddleware>();
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApplicationException ex)
            {
                await HandleExceptionAsync(context, ex);
            }
            catch (Exception ex)
            {
                await LogFailedRequestAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var responseMessage = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    responseMessage = JsonSerializer.Serialize(validationException.Errors);
                    break;
                case BadRequestException:
                    httpStatusCode = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException:
                    httpStatusCode = HttpStatusCode.NotFound;
                    break;
                case ForbiddenException:
                    httpStatusCode = HttpStatusCode.Forbidden;
                    break;
                case UnauthorizedException:
                    httpStatusCode = HttpStatusCode.Unauthorized;
                    break;
                case OperationCanceledException:
                    httpStatusCode = HttpStatusCode.Conflict;
                    break;
            }

            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = (int)httpStatusCode;

            if (responseMessage == string.Empty)
                responseMessage = JsonSerializer.Serialize(exception.Message);

            return context.Response.WriteAsync(responseMessage);
        }
        private async Task LogFailedRequestAsync(HttpContext context, Exception exception)
        {
            var httpStatusCode = HttpStatusCode.InternalServerError;
            var responseMessage = string.Empty;

            if (_hostingEnvironment.IsProduction())
            {
                _logger.LogCritical(exception, "Internal server error happened.");
                context.Response.ContentType = MediaTypeNames.Application.Json;
                responseMessage = JsonSerializer.Serialize("Internal server error happened.");
            }
            else
            {
                _logger.LogCritical(responseMessage);
                context.Response.ContentType = MediaTypeNames.Text.Plain;
                responseMessage = GetFailedRequestMessage(context, exception);
            }

            context.Response.StatusCode = (int)httpStatusCode;
            await context.Response.WriteAsync(responseMessage);
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

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }
}
