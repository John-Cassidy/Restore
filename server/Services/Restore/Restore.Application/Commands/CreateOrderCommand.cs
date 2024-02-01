using MediatR;
using Restore.Application.Requests;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class CreateOrderCommand : IRequest<Result<OrderResponse>>
{
    public string BuyerId { get; }
    public string UserName { get; }
    public AddressRequest ShippingAddress { get; }
    public bool SaveAddress { get; }

    public CreateOrderCommand(string buyerId, string userName, CreateOrderCommandRequest createOrder)
    {
        BuyerId = buyerId;
        UserName = userName;
        ShippingAddress = createOrder.ShippingAddress;
        SaveAddress = createOrder.SaveAddress;
    }
}