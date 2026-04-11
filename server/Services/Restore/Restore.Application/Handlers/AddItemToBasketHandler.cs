using MediatR;
using Restore.Application.Commands;
using Restore.Application.Mappers;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Result<BasketResponse>> {
    private readonly IBasketRepository _basketRepository;

    public AddItemToBasketHandler(IBasketRepository basketRepository) {
        _basketRepository = basketRepository;
    }

    public async Task<Result<BasketResponse>> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken) {
        var result = await _basketRepository.AddItemToBasketAsync(command.BuyerId, command.ProductId, command.Quantity);

        // If the result is a failure, return a NotFoundResult with the error message
        if (!result.IsSuccess) {
            // 1. Product not found
            // 2. Failed to add item to basket
            return Result<BasketResponse>.Failure(result.ErrorMessage);
        }

        // If the result is a success, map the result to a BasketResponse and return it
        return Result<BasketResponse>.Success(result.Value.ToBasketResponse());
    }
}