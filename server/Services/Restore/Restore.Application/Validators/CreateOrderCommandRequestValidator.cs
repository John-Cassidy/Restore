using FluentValidation;
using Restore.Application.Commands;

namespace Restore.Application.Validators;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.ShippingAddress).NotNull();
        RuleFor(x => x.ShippingAddress).SetValidator(new AddressRequestValidator());
    }
}