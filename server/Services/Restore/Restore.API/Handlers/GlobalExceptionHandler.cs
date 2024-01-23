using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Restore.Core.Exceptions;

namespace Restore.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
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

        var response = new ProblemDetails
        (
            statusCode,
            _env.IsDevelopment() ? exception.StackTrace?.ToString() : null,
            errorMessage
        );
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await httpContext.Response.WriteAsync(json);

        // await httpContext.Response.WriteAsJsonAsync(errorMessage);

        return true;
        // return new ValueTask<bool>(true);
    }
}

internal class ProblemDetails(int status, string? detail, string title)
{
    public int Status => status;
    public string? Detail => detail;
    public string Title => title;
}