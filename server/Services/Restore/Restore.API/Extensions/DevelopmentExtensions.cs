using FluentValidation;
using FluentValidation.Results;
using Restore.Core.Exceptions;
using Restore.API.Handlers;

namespace Restore.API.Extensions;

public static class DevelopmentExtensions
{
    public static void UseExceptionEndpoints(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapGet("/api/throwBadHttpRequest", (_) => throw new BadHttpRequestException("Bad request"));

            app.MapGet("/api/throwBadRequest", (_) => throw new BadRequestException("Bad request"));
            app.MapGet("/api/throwUnauthorized", (_) => throw new UnauthorizedException("Unauthorized"));
            app.MapGet("/api/throwNotFound", (_) => throw new NotFoundException("Not found"));
            app.MapGet("/api/throwException", (_) => throw new Exception("Something went wrong"));

            app.MapGet("/api/validation-error", (IValidationExceptionHandler validationExceptionHandler) =>
            {
                var ex = new ValidationException("Validation error", new List<ValidationFailure>
                {
                                new ValidationFailure("Problem1", "Problem1 is required"),
                                new ValidationFailure("Problem2", "Problem2 is required")
                });

                var logger = app.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Validation error occurred");

                var problemDetails = validationExceptionHandler.Handle(ex);
                return Results.Problem(title: problemDetails.Title, statusCode: problemDetails.Status, detail: problemDetails.Detail);
            });
        }
    }
}
