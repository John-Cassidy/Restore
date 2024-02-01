using FluentValidation;
using Restore.Application.Requests;

namespace Restore.Application.Validators;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Address1).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address2).MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(200);
        RuleFor(x => x.State).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Zip).NotEmpty().MaximumLength(200).Matches(@"^\d{5}(?:[-\s]\d{4})?$").WithMessage("Invalid zip code format");
        RuleFor(x => x.Country).NotEmpty().MaximumLength(200);
    }
}
