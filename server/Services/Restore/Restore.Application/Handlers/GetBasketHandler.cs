using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetBasketHandler : IRequestHandler<GetBasketQuery, Result<BasketResponse>> {
    private readonly IBasketRepository _basketRepository;

    public GetBasketHandler(IBasketRepository basketRepository) {
        _basketRepository = basketRepository;
    }

    public async Task<Result<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken) {
        var result = await _basketRepository.GetBasketAsync(request.BuyerId);
        return Result<BasketResponse>.Success(result.Value.ToBasketResponse());
    }
}
