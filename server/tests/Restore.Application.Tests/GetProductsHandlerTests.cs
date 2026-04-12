using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;

namespace Restore.Application.Tests;

public class GetProductsHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryReturnsProducts_MapsToProductResponses()
    {
        var productParams = new ProductParams { PageNumber = 1, PageSize = 2 };
        var products = new List<Product> {
            new() {
                Id = 1,
                Name = "Boots",
                Description = "Waterproof boots",
                Price = 1099,
                PictureUrl = "images/boots.png",
                Type = "Shoes",
                Brand = "Restore",
                QuantityInStock = 5
            }
        };

        var pagedProducts = new PagedList<Product>(products, count: 1, pageNumber: 1, pageSize: 2);
        var repository = new StubProductRepository(pagedProducts);
        var handler = new GetProductsHandler(repository);

        var result = await handler.Handle(new GetProductsQuery(productParams), CancellationToken.None);

        Assert.Single(result.Data);
        Assert.Equal("Boots", result.Data[0].Name);
        Assert.Equal(1, result.MetaData.TotalCount);
        Assert.Equal(1, repository.GetProductsAsyncCalls);
    }

    [Fact]
    public async Task Handle_PassesRequestProductParamsToRepository()
    {
        var productParams = new ProductParams { SearchTerm = "boots" };
        var repository = new StubProductRepository(new PagedList<Product>(new List<Product>(), 0, 1, 6));
        var handler = new GetProductsHandler(repository);

        await handler.Handle(new GetProductsQuery(productParams), CancellationToken.None);

        Assert.Same(productParams, repository.CapturedProductParams);
    }

    private sealed class StubProductRepository : IProductRepository
    {
        private readonly PagedList<Product> _pagedProducts;

        public StubProductRepository(PagedList<Product> pagedProducts)
        {
            _pagedProducts = pagedProducts;
        }

        public int GetProductsAsyncCalls { get; private set; }
        public ProductParams? CapturedProductParams { get; private set; }

        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());

        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams)
        {
            GetProductsAsyncCalls++;
            CapturedProductParams = productParams;
            return Task.FromResult(_pagedProducts);
        }

        public Task<Product?> GetByIdAsync(int id) => Task.FromResult<Product?>(null);

        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() =>
            Task.FromResult((new List<string>(), new List<string>()));

        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);

        public Task AddAsync(Product product) => Task.CompletedTask;

        public Task UpdateAsync(Product product) => Task.CompletedTask;

        public Task DeleteAsync(Product product) => Task.CompletedTask;
    }
}