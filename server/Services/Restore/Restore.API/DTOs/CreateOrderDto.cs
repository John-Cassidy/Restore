namespace Restore.API.DTOs;

public record CreateOrderDto(bool SaveAddress, AddressDto ShippingAddress);