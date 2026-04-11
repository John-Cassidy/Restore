using Restore.Application.Requests;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Pagination;

namespace Restore.Application.Mappers;

public static class MappingExtensions
{
    public static ProductResponse ToProductResponse(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Description = product.Description,
        Price = product.Price,
        PictureUrl = product.PictureUrl,
        Type = product.Type,
        Brand = product.Brand,
        QuantityInStock = product.QuantityInStock
    };

    public static PagedList<ProductResponse> ToProductResponsePagedList(this PagedList<Product> pagedList) => new(
        pagedList.Data.Select(p => p.ToProductResponse()).ToList(),
        pagedList.MetaData.TotalCount,
        pagedList.MetaData.CurrentPage,
        pagedList.MetaData.PageSize
    );

    public static BasketItemResponse ToBasketItemResponse(this BasketItem item) => new()
    {
        Id = item.Id,
        Quantity = item.Quantity,
        Product = item.Product.ToProductResponse()
    };

    public static BasketResponse ToBasketResponse(this Basket basket) => new()
    {
        Id = basket.Id,
        BuyerId = basket.BuyerId,
        Items = basket.Items.Select(i => i.ToBasketItemResponse()).ToList(),
        PaymentIntentId = basket.PaymentIntentId,
        ClientSecret = basket.ClientSecret
    };

    public static ProductItemOrderedResponse ToProductItemOrderedResponse(this ProductItemOrdered item) => new()
    {
        ProductId = item.ProductId,
        Name = item.Name,
        PictureUrl = item.PictureUrl
    };

    public static OrderItemResponse ToOrderItemResponse(this OrderItem item) => new()
    {
        Id = item.Id,
        ItemOrdered = item.ItemOrdered.ToProductItemOrderedResponse(),
        Price = item.Price,
        Quantity = item.Quantity
    };

    public static AddressResponse ToAddressResponse(this ShippingAddress address) => new()
    {
        FullName = address.FullName,
        Address1 = address.Address1,
        Address2 = address.Address2,
        City = address.City,
        State = address.State,
        Zip = address.Zip,
        Country = address.Country
    };

    public static OrderStatusResponse ToOrderStatusResponse(this OrderStatus status) =>
        (OrderStatusResponse)(int)status;

    public static OrderResponse ToOrderResponse(this Order order) => new()
    {
        Id = order.Id,
        BuyerId = order.BuyerId,
        ShippingAddress = order.ShippingAddress.ToAddressResponse(),
        OrderDate = order.OrderDate,
        OrderItems = order.OrderItems.Select(i => i.ToOrderItemResponse()).ToList(),
        Subtotal = order.Subtotal,
        DeliveryFee = order.DeliveryFee,
        OrderStatus = order.OrderStatus.ToOrderStatusResponse(),
        Total = order.GetTotal()
    };

    public static ShippingAddress ToShippingAddress(this AddressRequest address) => new()
    {
        FullName = address.FullName,
        Address1 = address.Address1,
        Address2 = address.Address2,
        City = address.City,
        State = address.State,
        Zip = address.Zip,
        Country = address.Country
    };

    public static void UpdateFrom(this Product target, Product source)
    {
        target.Name = source.Name;
        target.Description = source.Description;
        target.Price = source.Price;
        target.PictureUrl = source.PictureUrl;
        target.Type = source.Type;
        target.Brand = source.Brand;
        target.QuantityInStock = source.QuantityInStock;
    }

    public static void UpdateFrom(this Basket target, Basket source)
    {
        target.BuyerId = source.BuyerId;
        target.PaymentIntentId = source.PaymentIntentId;
        target.ClientSecret = source.ClientSecret;
    }

    public static void UpdateFrom(this Order target, Order source)
    {
        target.BuyerId = source.BuyerId;
        target.ShippingAddress = source.ShippingAddress;
        target.OrderDate = source.OrderDate;
        target.OrderItems = source.OrderItems;
        target.Subtotal = source.Subtotal;
        target.DeliveryFee = source.DeliveryFee;
        target.OrderStatus = source.OrderStatus;
        target.PaymentIntentId = source.PaymentIntentId;
    }
}
