using MediatR;
using Restore.Application.Commands;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class UpdateBasketHandler : IRequestHandler<UpdateBasketCommand, Result<bool>>
{
    private readonly IBasketRepository _basketRepository;

    public UpdateBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<bool>> Handle(UpdateBasketCommand command, CancellationToken cancellationToken)
    {
        Result<bool> result = await _basketRepository.UpdateBasketAsync(command.BuyerId, command.Username);

        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage);
        }

        return Result<bool>.Success(true);
    }
}
