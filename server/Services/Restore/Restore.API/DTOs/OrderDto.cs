namespace Restore.API.DTOs;

public record OrderDto(int Id, string BuyerId, AddressDto ShippingAddress, DateTime OrderDate, IReadOnlyList<OrderItemDto> OrderItems, long Subtotal, long DeliveryFee, string OrderStatus, long Total);

public record OrderItemDto(int ProductId, string Name, string PictureUrl, long Price, int Quantity);
