using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Restore.Core;

namespace Restore.API.Handlers;

public interface IValidationExceptionHandler
{
    ProblemDetails Handle(ValidationException ex);
    ValidationException CreateValidationExceptionFromErrorMessage(string errorMessage);
}

public class ValidationExceptionHandler : IValidationExceptionHandler
{
    public ProblemDetails Handle(ValidationException ex)
    {
        var validationResult = new ValidationResult(ex.Errors);
        if (validationResult.Errors.Count == 0)
        {
            return new ProblemDetails(StatusCodes.Status400BadRequest, ex.Message);
        }
        // var validationResults = validationResult.ToDictionary();
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var jsonString = JsonSerializer.Serialize(validationResult, options);

        return new ProblemDetails(StatusCodes.Status400BadRequest, "Validation failed", detail: jsonString);
    }

    public ValidationException CreateValidationExceptionFromErrorMessage(string errorMessage)
    {
        var errorMessages = errorMessage.Split(", ");
        var validationFailures = new List<ValidationFailure>();

        foreach (var message in errorMessages)
        {
            var parts = message.Split(": ");
            var failure = new ValidationFailure(parts[0], parts[1]);
            validationFailures.Add(failure);
        }

        var validationException = new ValidationException("Validation Errors", validationFailures);

        return validationException;
    }
}
