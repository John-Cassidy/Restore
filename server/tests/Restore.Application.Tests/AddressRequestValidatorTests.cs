using Restore.Application.Requests;
using Restore.Application.Validators;

namespace Restore.Application.Tests;

public class AddressRequestValidatorTests
{
    private readonly AddressRequestValidator _validator = new();

    private static AddressRequest ValidAddress => new(
        "John Doe", "123 Main St", "Apt 4", "Portland", "OR", "97201", "US");

    [Fact]
    public void Validate_ValidAddress_IsValid()
    {
        var result = _validator.Validate(ValidAddress);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyFullName_IsInvalid()
    {
        var address = new AddressRequest("", "123 Main St", "", "Portland", "OR", "97201", "US");
        var result = _validator.Validate(address);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_EmptyAddress1_IsInvalid()
    {
        var address = new AddressRequest("John Doe", "", "", "Portland", "OR", "97201", "US");
        var result = _validator.Validate(address);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Address1");
    }

    [Fact]
    public void Validate_InvalidZipFormat_IsInvalid()
    {
        var address = new AddressRequest("John Doe", "123 Main St", "", "Portland", "OR", "ABC", "US");
        var result = _validator.Validate(address);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Zip");
    }

    [Fact]
    public void Validate_ZipWithPlusFour_IsValid()
    {
        var address = new AddressRequest("John Doe", "123 Main St", "", "Portland", "OR", "97201-1234", "US");
        var result = _validator.Validate(address);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_EmptyCity_IsInvalid()
    {
        var address = new AddressRequest("John Doe", "123 Main St", "", "", "OR", "97201", "US");
        var result = _validator.Validate(address);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "City");
    }
}
