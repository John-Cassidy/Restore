namespace Restore.API.DTOs;

public record BasketDto(int Id, string BuyerId, List<BasketItemDto> Items, string? PaymentIntentId = null, string? ClientSecret = null);

public record BasketItemDto(int ProductId, string Name, long Price, string PictureUrl, string Brand, string Type, int Quantity);