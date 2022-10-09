using API.Contracts.DTOs;
using API.Exceptions;
using API.Helpers.CustomExceptionHandler.Abstracts;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class ForbiddenExceptionHandler : IExceptionHandler
    {
        public ExceptionResultDto Handle(ApplicationException exception)
        {
            var forbiddenException = (ForbiddenException)exception;

            return new ExceptionResultDto(MediaTypeNames.Application.Json, (int)HttpStatusCode.Forbidden, JsonSerializer.Serialize(forbiddenException.Message));
        }
    }
}
