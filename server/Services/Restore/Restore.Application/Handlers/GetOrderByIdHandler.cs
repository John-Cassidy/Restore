using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;

    public GetOrderByIdHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.BuyerId, request.OrderId);
        if (!order.IsSuccess)
        {
            return Result<OrderResponse>.Failure("Order not found");
        }
        return Result<OrderResponse>.Success(order.Value.ToOrderResponse());
    }
}
