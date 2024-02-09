using AutoMapper;
using MediatR;
using Restore.Application.Commands;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Result<BasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;

    public AddItemToBasketHandler(IBasketRepository basketRepository, IMapper mapper)
    {
        _basketRepository = basketRepository;
        _mapper = mapper;
    }

    public async Task<Result<BasketResponse>> Handle(AddItemToBasketCommand command, CancellationToken cancellationToken)
    {
        var result = await _basketRepository.AddItemToBasketAsync(command.BuyerId, command.ProductId, command.Quantity);

        // If the result is a failure, return a NotFoundResult with the error message
        if (!result.IsSuccess)
        {
            // 1. Product not found
            // 2. Failed to add item to basket
            return Result<BasketResponse>.Failure(result.ErrorMessage);
        }

        // If the result is a success, map the result to a BasketResponse and return it
        var basketResponse = _mapper.Map<BasketResponse>(result.Value);

        return Result<BasketResponse>.Success(basketResponse);
    }
}