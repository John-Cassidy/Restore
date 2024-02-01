using FluentValidation;
using Restore.Application.Requests;

namespace Restore.Application.Validators;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Address1).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.State).NotEmpty();
        RuleFor(x => x.Zip).NotEmpty();
        RuleFor(x => x.Country).NotEmpty();
    }
}
