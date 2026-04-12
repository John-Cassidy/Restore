using Restore.Core.Entities;

namespace Restore.Core.Tests;

public class BasketTests
{
    [Fact]
    public void AddItem_WhenProductDoesNotExist_AddsNewBasketItem()
    {
        var basket = new Basket();
        var product = CreateProduct(id: 5);

        basket.AddItem(product, quantity: 2);

        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items[0].Quantity);
    }

    [Fact]
    public void AddItem_WhenExistingItemHasSameProductId_IncreasesQuantity()
    {
        var basket = new Basket();
        var product = CreateProduct(id: 5);
        basket.Items.Add(new BasketItem
        {
            ProductId = 5,
            Product = product,
            Quantity = 1
        });

        basket.AddItem(product, quantity: 3);

        Assert.Single(basket.Items);
        Assert.Equal(4, basket.Items[0].Quantity);
    }

    [Fact]
    public void RemoveItem_WhenQuantityReachesZero_RemovesItem()
    {
        var basket = new Basket();
        basket.Items.Add(new BasketItem
        {
            ProductId = 5,
            Product = CreateProduct(id: 5),
            Quantity = 1
        });

        basket.RemoveItem(5, quantity: 1);

        Assert.Empty(basket.Items);
    }

    [Fact]
    public void RemoveItem_WhenQuantityRemains_KeepsItemWithUpdatedQuantity()
    {
        var basket = new Basket();
        basket.Items.Add(new BasketItem
        {
            ProductId = 5,
            Product = CreateProduct(id: 5),
            Quantity = 4
        });

        basket.RemoveItem(5, quantity: 1);

        Assert.Single(basket.Items);
        Assert.Equal(3, basket.Items[0].Quantity);
    }

    private static Product CreateProduct(int id) => new()
    {
        Id = id,
        Name = "Boots",
        Description = "All weather",
        Price = 1000,
        PictureUrl = "images/boot.png",
        Type = "Shoes",
        Brand = "Restore",
        QuantityInStock = 10
    };
}