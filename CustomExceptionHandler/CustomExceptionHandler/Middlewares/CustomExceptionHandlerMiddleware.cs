using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CustomExceptionHandler.Middlewares
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<CustomExceptionHandlerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                LogFailedRequest(context, ex);

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;

            var result = string.Empty;

            //switch (exception)
            //{
            //    case ValidationException validationException:
            //        code = HttpStatusCode.BadRequest;
            //        result = JsonSerializer.Serialize(validationException.Failures);
            //        break;
            //    case BadRequestException badRequestException:
            //        code = HttpStatusCode.BadRequest;
            //        result = badRequestException.Message;
            //        break;
            //    case NotFoundException _:
            //        code = HttpStatusCode.NotFound;
            //        break;
            //    case UnauthorizedException _:
            //        code = HttpStatusCode.Unauthorized;
            //        break;
            //}

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (result == string.Empty) result = JsonSerializer.Serialize(new { error = exception.Message });

            return context.Response.WriteAsync(result);
        }

        private void LogFailedRequest(HttpContext context, Exception exception)
        {
            var user = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";

            _logger.LogError(
                "Failed Request\n" +
                "\tSchema: {Schema}\n" +
                "\tHost: {Host}\n" +
                "\tUser: {User}\n" +
                "\tMethod: {Method}\n" +
                "\tPath: {Path}\n" +
                "\tQueryString: {QueryString}\n" +
                "\tErrorMessage: {ErrorMessage}\n" +
                "\tStacktrace (5):\n{StackTrace}",
                context.Request?.Scheme,
                context.Request?.Host,
                user,
                context.Request?.Method,
                context.Request?.Path,
                context.Request?.QueryString,
                exception.Message,
                exception.StackTrace?.Split('\n').Take(5).Aggregate((a, b) => a + "\n" + b)
            );
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
