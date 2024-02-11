using Restore.API.DTOs;
using Restore.Application.Responses;

namespace Restore.API.Extensions;

public static class BasketExtensions
{
    public static BasketDto MapBasketToDto(this BasketResponse basket)
    {
        return new BasketDto(
            basket.Id,
            basket.BuyerId,
            basket.Items.Select(item => new BasketItemDto(
                item.Product.Id, // Fix the property name to access ProductId
                item.Product.Name,
                item.Product.Price,
                item.Product.PictureUrl,
                item.Product.Brand,
                item.Product.Type,
                item.Quantity
            )).ToList(),
            basket.PaymentIntentId,
            basket.ClientSecret
        );
    }
}
