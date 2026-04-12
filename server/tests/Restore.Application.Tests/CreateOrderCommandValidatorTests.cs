using Restore.Application.Commands;
using Restore.Application.Requests;
using Restore.Application.Validators;

namespace Restore.Application.Tests;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator = new();

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        var address = new AddressRequest("John Doe", "123 Main St", "", "Portland", "OR", "97201", "US");
        var request = new CreateOrderCommandRequest(false, address);
        var command = new CreateOrderCommand("buyer1", "john", request);

        var result = _validator.Validate(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NullShippingAddress_IsInvalid()
    {
        var command = new CreateOrderCommand("buyer1", "john",
            new CreateOrderCommandRequest(false, null!));

        var result = _validator.Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ShippingAddress");
    }
}
