using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class AddItemToBasketHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryFails_ReturnsFailure()
    {
        var repository = new StubBasketRepository
        {
            AddItemResult = Result<Basket>.Failure("Product not found")
        };
        var handler = new AddItemToBasketHandler(repository);

        var result = await handler.Handle(
            new AddItemToBasketCommand("buyer-1", 99, 2), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenRepositorySucceeds_ReturnsMappedBasketResponse()
    {
        var basket = new Basket { BuyerId = "buyer-1" };
        basket.Items.Add(new BasketItem
        {
            ProductId = 5,
            Quantity = 2,
            Product = new Product
            {
                Id = 5,
                Name = "Hat",
                Description = "Cool hat",
                Price = 1500,
                PictureUrl = "img.png",
                Type = "Hats",
                Brand = "Restore",
                QuantityInStock = 10
            }
        });
        var repository = new StubBasketRepository
        {
            AddItemResult = Result<Basket>.Success(basket)
        };
        var handler = new AddItemToBasketHandler(repository);

        var result = await handler.Handle(
            new AddItemToBasketCommand("buyer-1", 5, 2), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("buyer-1", result.Value.BuyerId);
        Assert.Single(result.Value.Items);
        Assert.Equal("Hat", result.Value.Items[0].Product.Name);
    }

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Result<Basket> AddItemResult { get; set; } = Result<Basket>.Failure("not configured");

        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) =>
            Task.FromResult(AddItemResult);

        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult<Basket?>(null);
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;
        public Task DeleteAsync(Basket basket) => Task.CompletedTask;
    }
}
