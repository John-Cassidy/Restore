using Microsoft.AspNetCore.Diagnostics;
using Restore.Core.Exceptions;

namespace Restore.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int statusCode, string errorMessage) = exception switch
        {
            InvalidCastException invalidCastException => (StatusCodes.Status400BadRequest, invalidCastException.Message),
            AggregateException aggregateException => (StatusCodes.Status400BadRequest, aggregateException.Message),
            ArgumentNullException argumentNullException => (StatusCodes.Status400BadRequest, argumentNullException.Message),
            ArgumentException argumentException => (StatusCodes.Status400BadRequest, argumentException.Message),
            // ValidationException validationException => (StatusCodes.Status400BadRequest, validationException.Message),
            KeyNotFoundException keyNotFoundException => (StatusCodes.Status400BadRequest, keyNotFoundException.Message),
            FormatException formatException => (StatusCodes.Status400BadRequest, formatException.Message),
            // ForbidException forbidException => (StatusCodes.Status403Forbidden, "Forbidden"),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Bad request"),
            NotFoundException notFoundException => (StatusCodes.Status404NotFound, notFoundException.Message),
            _ => default
        };

        if (statusCode == default)
        {
            return false;
        }

        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(errorMessage);

        return true;
        // return new ValueTask<bool>(true);
    }
}
