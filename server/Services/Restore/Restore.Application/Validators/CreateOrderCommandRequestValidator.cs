using FluentValidation;
using Restore.Application.Requests;

namespace Restore.Application.Validators;

public class CreateOrderCommandRequestValidator : AbstractValidator<CreateOrderCommandRequest>
{
    public CreateOrderCommandRequestValidator()
    {
        RuleFor(x => x.ShippingAddress).NotNull();
        RuleFor(x => x.ShippingAddress).SetValidator(new AddressRequestValidator());
    }
}