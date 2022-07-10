using System;

namespace API.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string name, object key)
            : base($"{name} with id: ({key}) not found in database")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
