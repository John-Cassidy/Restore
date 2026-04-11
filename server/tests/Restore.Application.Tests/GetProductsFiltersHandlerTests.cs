using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;

namespace Restore.Application.Tests;

public class GetProductsFiltersHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsFilters_MapsToResponse()
    {
        var repository = new StubProductRepository
        {
            Filters = (new List<string> { "Restore" }, new List<string> { "Shoes" })
        };
        var handler = new GetProductsFiltersHandler(repository);

        var response = await handler.Handle(new GetProductsFiltersQuery(), CancellationToken.None);

        Assert.Single(response.Brands);
        Assert.Single(response.Types);
        Assert.Equal("Restore", response.Brands[0]);
        Assert.Equal("Shoes", response.Types[0]);
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public (List<string> Brands, List<string> Types) Filters { get; set; } = (new List<string>(), new List<string>());

        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams) =>
            Task.FromResult(new PagedList<Product>(new List<Product>(), 0, 1, 6));
        public Task<Product?> GetByIdAsync(int id) => Task.FromResult<Product?>(null);
        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() => Task.FromResult(Filters);
        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;
        public Task DeleteAsync(Product product) => Task.CompletedTask;
    }
}