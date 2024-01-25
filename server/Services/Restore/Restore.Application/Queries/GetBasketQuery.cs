using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Queries;

public class GetBasketQuery : IRequest<Result<BasketResponse>>
{
    public GetBasketQuery(string buyerId)
    {
        BuyerId = buyerId;
    }

    public string BuyerId { get; }
}