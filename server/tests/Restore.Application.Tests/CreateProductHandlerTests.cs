using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Application.Services;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_WhenImageUploadFails_ReturnsFailure()
    {
        var unitOfWork = new StubUnitOfWork { CompleteResult = 1 };
        var imageService = new StubImageService { AddResult = Result<string>.Failure("upload failed") };
        var handler = new CreateProductHandler(unitOfWork, imageService);
        var command = MakeCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("upload failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenDatabaseSaveFails_ReturnsFailure()
    {
        var unitOfWork = new StubUnitOfWork { CompleteResult = 0 };
        var imageService = new StubImageService { AddResult = Result<string>.Success("images/new.png") };
        var handler = new CreateProductHandler(unitOfWork, imageService);
        var command = MakeCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to create product", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenSucceeds_ReturnsMappedProductResponse()
    {
        var unitOfWork = new StubUnitOfWork { CompleteResult = 1 };
        var imageService = new StubImageService { AddResult = Result<string>.Success("images/new.png") };
        var handler = new CreateProductHandler(unitOfWork, imageService);
        var command = MakeCommand();

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Test Product", result.Value.Name);
        Assert.Equal("images/new.png", result.Value.PictureUrl);
        Assert.Equal(2500, result.Value.Price);
    }

    private static CreateProductCommand MakeCommand() =>
        new("Test Product", "A description", 2500, "Boots", "Restore", 10, new StubFormFileService());

    private sealed class StubFormFileService : IFormFileService
    {
        public string FileName => "test.png";
        public long Length => 100;
        public string ContentType => "image/png";
        public Stream OpenReadStream() => Stream.Null;
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public int CompleteResult { get; set; }
        public IBasketRepository BasketRepository { get; } = null!;
        public IOrderRepository OrderRepository { get; } = null!;
        public IProductRepository ProductRepository { get; } = new StubProductRepository();
        public IUserRepository UserRepository { get; } = null!;
        public Task<int> CompleteAsync() => Task.FromResult(CompleteResult);
        public void Dispose() { }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams) =>
            Task.FromResult(new PagedList<Product>(new List<Product>(), 0, 1, 6));
        public Task<Product?> GetByIdAsync(int id) => Task.FromResult<Product?>(null);
        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() => Task.FromResult((new List<string>(), new List<string>()));
        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;
        public Task DeleteAsync(Product product) => Task.CompletedTask;
    }

    private sealed class StubImageService : IImageService
    {
        public Result<string> AddResult { get; set; } = Result<string>.Failure("not configured");
        public Task<Result<string>> AddImageAsync(IFormFileService formFileService) => Task.FromResult(AddResult);
        public Task<Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl) => Task.FromResult(Result<string>.Failure("not used"));
        public Task<Result<bool>> DeleteImageAsync(string imagePath) => Task.FromResult(Result<bool>.Failure("not used"));
    }
}
