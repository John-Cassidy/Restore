using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;

namespace Restore.Application.Tests;

public class GetProductByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductExists_ReturnsMappedResponse()
    {
        var product = new Product
        {
            Id = 42,
            Name = "Running Shoes",
            Description = "Lightweight",
            Price = 5999,
            PictureUrl = "images/running.png",
            Type = "Shoes",
            Brand = "Restore",
            QuantityInStock = 12
        };

        var repository = new StubProductRepository { ProductById = product };
        var handler = new GetProductByIdHandler(repository);

        var response = await handler.Handle(new GetProductByIdQuery(42), CancellationToken.None);

        Assert.Equal(42, response.Id);
        Assert.Equal("Running Shoes", response.Name);
        Assert.Equal(1, repository.GetByIdCallCount);
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Product? ProductById { get; set; }
        public int GetByIdCallCount { get; private set; }

        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());

        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams) =>
            Task.FromResult(new PagedList<Product>(new List<Product>(), 0, 1, 6));

        public Task<Product?> GetByIdAsync(int id)
        {
            GetByIdCallCount++;
            return Task.FromResult(ProductById);
        }

        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() =>
            Task.FromResult((new List<string>(), new List<string>()));

        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;
        public Task DeleteAsync(Product product) => Task.CompletedTask;
    }
}