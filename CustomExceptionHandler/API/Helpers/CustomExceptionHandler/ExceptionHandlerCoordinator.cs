using API.Contracts.DTOs;
using API.Exceptions;
using API.Helpers.CustomExceptionHandler.Abstracts;
using API.Helpers.CustomExceptionHandler.Concretes;

namespace API.Helpers.CustomExceptionHandler
{
    public class ExceptionHandlerCoordinator
    {
        private Dictionary<Type, IExceptionHandler> _exceptionHandlers = new Dictionary<Type, IExceptionHandler>();

        public ExceptionHandlerCoordinator(
            ValidationExceptionHandler validationExceptionHandler,
            BadRequestExceptionHandler badRequestExceptionHandler,
            NotFoundExceptionHandler notFoundExceptionHandler,
            ForbiddenExceptionHandler forbiddenExceptionHandler,
            UnauthorizedExceptionHandler unauthorizedExceptionHandler,
            GeneralExceptionHandler generalExceptionHandler)
        {
            RegisterHandler<ValidationException>(validationExceptionHandler);
            RegisterHandler<BadRequestException>(badRequestExceptionHandler);
            RegisterHandler<NotFoundException>(notFoundExceptionHandler);
            RegisterHandler<ForbiddenException>(forbiddenExceptionHandler);
            RegisterHandler<UnauthorizedException>(unauthorizedExceptionHandler);
            RegisterHandler<Exception>(generalExceptionHandler);
        }

        public void RegisterHandler<TException>(IExceptionHandler handler)
            where TException : Exception
        {
            ArgumentNullException.ThrowIfNull(handler);

            _exceptionHandlers[typeof(TException)] = handler;
        }

        public ExceptionResultDto Handle(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return _exceptionHandlers[exception.GetType()].Handle(exception);
        }
    }
}
