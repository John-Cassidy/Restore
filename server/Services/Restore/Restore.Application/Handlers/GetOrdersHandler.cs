using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<IReadOnlyList<OrderResponse>>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrdersHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<IReadOnlyList<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderList = await _orderRepository.GetOrdersAsync(request.BuyerId);
        if (!orderList.IsSuccess)
        {
            return Result<IReadOnlyList<OrderResponse>>.Failure("Orders not found");
        }
        var orderResponseList = orderList.Value.Select(o => o.ToOrderResponse()).ToList();
        return Result<IReadOnlyList<OrderResponse>>.Success(orderResponseList);
    }
}
