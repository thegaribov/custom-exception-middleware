using API.Exceptions;
using API.Helpers.CustomExceptionHandler;
using API.Helpers.CustomExceptionHandler.Concretes;
using API.Middlewares;
using FluentValidation.AspNetCore;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddFluentValidation(fvc => fvc.RegisterValidatorsFromAssemblyContaining<Program>());
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped<BadRequestExceptionHandler>();
            builder.Services.AddScoped<ForbiddenExceptionHandler>();
            builder.Services.AddScoped<GeneralExceptionHandler>();
            builder.Services.AddScoped<NotFoundExceptionHandler>();
            builder.Services.AddScoped<UnauthorizedExceptionHandler>();
            builder.Services.AddScoped<ValidationExceptionHandler>();

            builder.Services.AddScoped<ExceptionHandlerCoordinator>();

            var app = builder.Build();

            app.UseCustomExceptionHandler();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapGet("/not-found-example", () =>
            {
                throw new NotFoundException("Information is not found in DB");
            });

            app.MapGet("/bad-request-example", () =>
            {
                throw new BadRequestException("Requester URL is invalid");
            });

            app.MapGet("/forbidden-request-example", () =>
            {
                throw new ForbiddenException("Requester URL is invalid");
            });

            app.MapGet("/unauthorized-request-example", () =>
            {
                throw new UnauthorizedException("You don't have access to this module");
            });

            app.MapGet("/validation-request-example", () =>
            {
                throw new ValidationException("Only only admin is allowed");
            });

            app.MapGet("/exception", () =>
            {
                throw new Exception("Something went wrong");
            });

            app.MapGet("/unregistered-exception", () =>
            {
                throw new MyException();
            });

            //app.MapControllers();

            app.Run();
        }
    }
}