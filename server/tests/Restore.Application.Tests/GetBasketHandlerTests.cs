using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class GetBasketHandlerTests
{
    [Fact]
    public async Task Handle_WhenBasketExists_ReturnsMappedBasketResponse()
    {
        var repository = new StubBasketRepository
        {
            BasketResult = Result<Basket>.Success(CreateBasket())
        };
        var handler = new GetBasketHandler(repository);

        var result = await handler.Handle(new GetBasketQuery("buyer-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("buyer-1", result.Value.BuyerId);
        Assert.Single(result.Value.Items);
        Assert.Equal("Boots", result.Value.Items[0].Product.Name);
    }

    private static Basket CreateBasket() => new()
    {
        BuyerId = "buyer-1",
        Items = new List<BasketItem> {
            new() {
                ProductId = 5,
                Quantity = 2,
                Product = new Product {
                    Id = 5,
                    Name = "Boots",
                    Description = "All weather",
                    Price = 5000,
                    PictureUrl = "images/boots.png",
                    Type = "Shoes",
                    Brand = "Restore",
                    QuantityInStock = 4
                }
            }
        }
    };

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Result<Basket> BasketResult { get; set; } = Result<Basket>.Failure("not used");

        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(BasketResult);
        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult<Basket?>(null);
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;
        public Task DeleteAsync(Basket basket) => Task.CompletedTask;
    }
}