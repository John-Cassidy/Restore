using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Restore.Core;

namespace Restore.API.Handlers;

public class GeneralExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GeneralExceptionHandler> _logger;
    private readonly IHostEnvironment _env;

    public GeneralExceptionHandler(ILogger<GeneralExceptionHandler> logger, IHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var response = new ProblemDetails
        (
            StatusCodes.Status500InternalServerError,
            _env.IsDevelopment() ? exception.StackTrace?.ToString() : null,
            exception.Message
        );
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);

        await httpContext.Response.WriteAsync(json);
        // await httpContext.Response.WriteAsync("Something went wrong..");

        return true;
    }
}
