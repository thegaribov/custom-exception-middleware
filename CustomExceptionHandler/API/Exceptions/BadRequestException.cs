using System;

namespace API.Exceptions
{
    public class BadRequestException : ApplicationException
    {
        public BadRequestException(string message)
            : base(message)
        {

        }
    }
}
