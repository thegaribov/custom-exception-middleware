using API.Contracts.DTOs;
using API.Helpers.CustomExceptionHandler.Abstracts;

namespace API.Helpers.CustomExceptionHandler.Concretes
{
    public class ExceptionHandlerService
    {
        private readonly Dictionary<Type, Func<IExceptionHandler>> _exceptionHandlers;

        public ExceptionHandlerService()
        {
            _exceptionHandlers = new Dictionary<Type, Func<IExceptionHandler>>();
        }

        public void RegisterHandler<T>(Func<IExceptionHandler> handler)
            where T : ApplicationException
        {
            ArgumentNullException.ThrowIfNull(handler);

            _exceptionHandlers[typeof(T)] = handler;
        }

        public ExceptionResultDto Handle(ApplicationException exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return _exceptionHandlers[exception.GetType()].Invoke().Handle(exception);
        }
    }
}
