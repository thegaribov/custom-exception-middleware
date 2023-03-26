using API.Contracts.DTOs;
using API.Exceptions;
using API.Helpers.CustomExceptionHandler.Abstracts;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class ValidationExceptionHandler : IExceptionHandler
    {
        public ExceptionResultDto Handle(Exception exception)
        {
            var validationException = (ValidationException)exception;

            return new ExceptionResultDto(MediaTypeNames.Application.Json, (int)HttpStatusCode.BadRequest, JsonSerializer.Serialize(validationException.Errors));
        }
    }
}
