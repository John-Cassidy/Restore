using MediatR;
using Microsoft.Extensions.Logging;

namespace Restore.Application.Behavior;

// create class that implements IPipelineBehavior<TRequest, TResponse> called UnhandledExceptionBehaviour where TRequest: IRequest<TResponse>

// inject ILogger<TRequest> into constructor
// implement Handle method
// use logger to log exception

public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            _logger.LogError(ex, $"Unhandled Exception Occurred with Request Name: {requestName}, {request}");

            throw;
        }
    }
}