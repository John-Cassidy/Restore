using System.Text.Json;
using FluentValidation;
using FluentValidation.Results;
using Restore.Core;

namespace Restore.API.Handlers;

public interface IValidationExceptionHandler
{
    ProblemDetails Handle(ValidationException ex);
}

public class ValidationExceptionHandler : IValidationExceptionHandler
{
    public ProblemDetails Handle(ValidationException ex)
    {
        var validationResult = new ValidationResult(ex.Errors);
        if (validationResult.Errors.Count == 0)
        {
            return new ProblemDetails(StatusCodes.Status400BadRequest, null, ex.Message);
        }
        var validationResults = validationResult.ToDictionary();
        var options = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        var jsonString = JsonSerializer.Serialize(validationResults, options);

        return new ProblemDetails(StatusCodes.Status400BadRequest, null, jsonString);
    }
}
