using AutoMapper;
using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Handlers;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<IReadOnlyList<OrderResponse>>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public GetOrdersHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }
    public async Task<Result<IReadOnlyList<OrderResponse>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orderList = await _orderRepository.GetOrdersAsync(request.BuyerId);
        if (!orderList.IsSuccess)
        {
            return Result<IReadOnlyList<OrderResponse>>.Failure("Orders not found");
        }
        var orderResponseList = _mapper.Map<IReadOnlyList<OrderResponse>>(orderList.Value);
        return Result<IReadOnlyList<OrderResponse>>.Success(orderResponseList);
    }
}
