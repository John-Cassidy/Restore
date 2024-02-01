using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Queries;

public class GetOrderByIdQuery : IRequest<Result<OrderResponse>>
{
    public string BuyerId { get; }
    public int OrderId { get; }

    public GetOrderByIdQuery(string buyerId, int orderId)
    {
        BuyerId = buyerId;
        OrderId = orderId;
    }
}
