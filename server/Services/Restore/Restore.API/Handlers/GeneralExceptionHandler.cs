using Microsoft.AspNetCore.Diagnostics;

namespace Restore.API.Handlers;

public class GeneralExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GeneralExceptionHandler> _logger;

    public GeneralExceptionHandler(ILogger<GeneralExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception occured: {Message}", exception.Message);
        httpContext.Response.StatusCode = StatusCodes.Status501NotImplemented;
        await httpContext.Response.WriteAsync("Something went wrong..");

        return true;
    }
}
