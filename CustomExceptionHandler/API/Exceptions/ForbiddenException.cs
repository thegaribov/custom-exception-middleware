using System;

namespace API.Exceptions
{
    public class ForbiddenException : ApplicationException
    {
        public ForbiddenException(string message)
            : base(message)
        {

        }
    }
}
