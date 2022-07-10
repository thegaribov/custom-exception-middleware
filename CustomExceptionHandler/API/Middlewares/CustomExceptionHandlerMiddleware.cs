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
            return "Failed Request\n" +
                $"\tSchema: {context.Request?.Scheme}\n" +
                $"\tHost: {context.Request?.Host}\n" +
                $"\tUser: {context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"}\n" +
                $"\tMethod: {context.Request?.Method}\n" +
                $"\tPath: {context.Request?.Path}\n" +
                $"\tQueryString: {context.Request?.QueryString}\n" +
                $"\tErrorMessage: {exception.Message}\n" +
                $"\tStacktrace (5):\n{exception.StackTrace?.Split('\n').Take(5).Aggregate((a, b) => a + "\n" + b)}";
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
