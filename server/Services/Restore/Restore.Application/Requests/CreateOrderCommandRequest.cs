using MediatR;

namespace Restore.Application.Requests;

public class CreateOrderCommandRequest : IRequest
{
    public bool SaveAddress { get; }
    public AddressRequest ShippingAddress { get; }

    public CreateOrderCommandRequest(bool saveAddress, AddressRequest shippingAddress)
    {
        SaveAddress = saveAddress;
        ShippingAddress = shippingAddress;
    }
}
