using System;

namespace CustomExceptionHandler.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message)
            : base(message)
        {

        }
    }
}
