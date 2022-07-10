using System;

namespace CustomExceptionHandler.Exceptions
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string message)
            : base(message)
        {

        }
    }
}
