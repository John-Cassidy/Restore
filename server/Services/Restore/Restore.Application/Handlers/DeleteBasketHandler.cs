using MediatR;
using Restore.Application.Commands;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class DeleteBasketHandler : IRequestHandler<DeleteBasketCommand, Result<bool>>
{
    private readonly IBasketRepository _basketRepository;

    public DeleteBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<bool>> Handle(DeleteBasketCommand command, CancellationToken cancellationToken)
    {
        Result<bool> result = await _basketRepository.DeleteBasketAsync(command.BuyerId);

        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage);
        }

        return Result<bool>.Success(true);
    }
}
