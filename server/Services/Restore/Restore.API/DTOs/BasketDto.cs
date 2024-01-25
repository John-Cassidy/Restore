using Restore.Application.Responses;
using System.Collections.Generic; // Add missing import statement

namespace Restore.API.DTOs;

public record BasketDto(int Id, string BuyerId, List<BasketItemDto> Items);

public record BasketItemDto(int ProductId, string Name, long Price, string PictureUrl, string Brand, string Type, int Quantity);