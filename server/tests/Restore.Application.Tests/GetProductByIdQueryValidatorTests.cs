using Restore.Application.Queries;
using Restore.Application.Validators;

namespace Restore.Application.Tests;

public class GetProductByIdQueryValidatorTests
{
    private readonly GetProductByIdQueryValidator _validator = new();

    [Fact]
    public void Validate_WhenIdIsGreaterThanZero_HasNoErrors()
    {
        var query = new GetProductByIdQuery(1);

        var result = _validator.Validate(query);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_WhenIdIsZero_HasValidationError()
    {
        var query = new GetProductByIdQuery(0);

        var result = _validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(GetProductByIdQuery.Id));
    }
}