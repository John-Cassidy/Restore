using FluentValidation;
using Restore.Application.Queries;

namespace Restore.Application.Validators;

public class GetProductByIdQueryValidator : AbstractValidator<GetProductByIdQuery>
{
    public GetProductByIdQueryValidator()
    {
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
