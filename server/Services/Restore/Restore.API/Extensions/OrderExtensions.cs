using Restore.API.DTOs;
using Restore.Application.Responses;

namespace Restore.API.Extensions;

public static class OrderExtensions
{

    public static OrderDto MapOrderToDto(this OrderResponse order)
    {
        return new OrderDto(order.Id, order.BuyerId,
                    order.ShippingAddress.MapAddressToDto(),
                    order.OrderDate,
                    order.OrderItems
                    .Select(item => item.MapOrderItemToDto()).ToList(),
                    order.Subtotal,
                    order.DeliveryFee,
                    order.OrderStatus.ToString(),
                    order.Total);
    }

    public static IReadOnlyList<OrderDto> MapOrdersToDto(this IReadOnlyList<OrderResponse> orders)
    {
        return orders.Select(order =>
            new OrderDto(order.Id, order.BuyerId,
                    order.ShippingAddress.MapAddressToDto(),
                    order.OrderDate,
                    order.OrderItems
                    .Select(item => item.MapOrderItemToDto()).ToList(),
                    order.Subtotal,
                    order.DeliveryFee,
                    order.OrderStatus.ToString(),
                    order.Total
        )).ToList();
    }

    public static AddressDto MapAddressToDto(this AddressResponse address)
    {
        return new AddressDto(address.FullName, address.Address1, address.Address2, address.City, address.State, address.Zip, address.Country);
    }

    public static OrderItemDto MapOrderItemToDto(this OrderItemResponse item)
    {
        return new OrderItemDto(item.ItemOrdered.ProductId, item.ItemOrdered.Name, item.ItemOrdered.PictureUrl, item.Price, item.Quantity);
    }

}
