using API.Contracts.DTOs;
using API.Exceptions;
using API.Helpers.CustomExceptionHandler.Abstracts;
using System.Net;
using System.Net.Mime;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class BadRequestExceptionHandler : IExceptionHandler
    {
        public ExceptionResultDto Handle(Exception exception)
        {
            var badRequestException = (BadRequestException)exception;

            return new ExceptionResultDto(MediaTypeNames.Text.Plain, (int)HttpStatusCode.BadRequest, badRequestException.Message);
        }
    }
}
