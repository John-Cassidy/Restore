using System.Diagnostics.CodeAnalysis;

namespace Restore.API.DTOs;

[ExcludeFromCodeCoverage]
public record CreateOrderDto(bool SaveAddress, AddressDto ShippingAddress);