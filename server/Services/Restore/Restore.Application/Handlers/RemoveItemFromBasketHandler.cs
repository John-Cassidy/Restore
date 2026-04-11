using MediatR;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class RemoveItemFromBasketHandler : IRequestHandler<RemoveItemFromBasketCommand, Result<bool>>
{
    private readonly IBasketRepository _basketRepository;

    public RemoveItemFromBasketHandler(IBasketRepository basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Result<bool>> Handle(RemoveItemFromBasketCommand request, CancellationToken cancellationToken)
    {
        var result = await _basketRepository.RemoveItemFromBasketAsync(request.BuyerId, request.ProductId, request.Quantity);

        if (!result.IsSuccess)
        {
            return Result<bool>.Failure(result.ErrorMessage);
        }

        return Result<bool>.Success(true);
    }
}
