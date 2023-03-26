using API.Contracts.DTOs;
using API.Exceptions;
using API.Helpers.CustomExceptionHandler.Abstracts;
using System.Net;
using System.Net.Mime;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class NotFoundExceptionHandler : IExceptionHandler
    {
        public ExceptionResultDto Handle(Exception exception)
        {
            var notFoundException = (NotFoundException)exception;

            return new ExceptionResultDto(MediaTypeNames.Text.Plain, (int)HttpStatusCode.NotFound, notFoundException.Message);
        }
    }
}
