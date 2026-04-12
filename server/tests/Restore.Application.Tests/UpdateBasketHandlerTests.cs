using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class UpdateBasketHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsFailure_ReturnsFailure()
    {
        var repository = new StubBasketRepository
        {
            UpdateResult = Result<bool>.Failure("basket missing")
        };
        var handler = new UpdateBasketHandler(repository);

        var result = await handler.Handle(new UpdateBasketCommand("buyer-1", "john"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("basket missing", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenRepositoryReturnsSuccess_ReturnsSuccess()
    {
        var repository = new StubBasketRepository
        {
            UpdateResult = Result<bool>.Success(true)
        };
        var handler = new UpdateBasketHandler(repository);

        var result = await handler.Handle(new UpdateBasketCommand("buyer-1", "john"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Result<bool> UpdateResult { get; set; } = Result<bool>.Success(true);

        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(UpdateResult);
        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult<Basket?>(null);
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;
        public Task DeleteAsync(Basket basket) => Task.CompletedTask;
    }
}