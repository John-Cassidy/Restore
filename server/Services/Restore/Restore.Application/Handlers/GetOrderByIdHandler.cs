using AutoMapper;
using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderResponse>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrderByIdHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<Result<OrderResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderByIdAsync(request.BuyerId, request.OrderId);
        if (!order.IsSuccess)
        {
            return Result<OrderResponse>.Failure("Order not found");
        }
        var orderResponse = _mapper.Map<OrderResponse>(order.Value);
        return Result<OrderResponse>.Success(orderResponse);
    }
}
