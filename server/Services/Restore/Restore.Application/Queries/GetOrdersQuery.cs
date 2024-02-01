using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Queries;

public class GetOrdersQuery : IRequest<Result<IReadOnlyList<OrderResponse>>>
{
    public string BuyerId { get; }

    public GetOrdersQuery(string buyerId)
    {
        BuyerId = buyerId;
    }
}
