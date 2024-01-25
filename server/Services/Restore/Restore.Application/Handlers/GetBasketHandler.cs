using AutoMapper;
using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetBasketHandler : IRequestHandler<GetBasketQuery, Result<BasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;

    public GetBasketHandler(IBasketRepository basketRepository, IMapper mapper)
    {
        _basketRepository = basketRepository;
        _mapper = mapper;
    }

    public async Task<Result<BasketResponse>> Handle(GetBasketQuery request, CancellationToken cancellationToken)
    {
        var result = await _basketRepository.GetBasketAsync(request.BuyerId);
        var basketResponse = _mapper.Map<BasketResponse>(result.Value);
        return Result<BasketResponse>.Success(basketResponse);
    }
}
