using API.Contracts.DTOs;

namespace API.Helpers.CustomExceptionHandler.Abstracts
{
    public interface IExceptionHandler
    {
        public ExceptionResultDto Handle(ApplicationException exception);
    }
}
