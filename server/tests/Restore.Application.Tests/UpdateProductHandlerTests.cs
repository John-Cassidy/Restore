using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Application.Services;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class UpdateProductHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsFailure()
    {
        var productRepo = new StubProductRepository { ProductById = null };
        var unitOfWork = new StubUnitOfWork(productRepo) { CompleteResult = 1 };
        var imageService = new StubImageService();
        var handler = new UpdateProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(MakeCommand(includeFile: false), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenFileProvidedAndImageUpdateFails_ReturnsFailure()
    {
        var productRepo = new StubProductRepository { ProductById = CreateProduct() };
        var unitOfWork = new StubUnitOfWork(productRepo) { CompleteResult = 1 };
        var imageService = new StubImageService { UpdateResult = Result<string>.Failure("update failed") };
        var handler = new UpdateProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(MakeCommand(includeFile: true), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("update failed", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenDatabaseSaveFails_ReturnsFailure()
    {
        var productRepo = new StubProductRepository { ProductById = CreateProduct() };
        var unitOfWork = new StubUnitOfWork(productRepo) { CompleteResult = 0 };
        var imageService = new StubImageService();
        var handler = new UpdateProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(MakeCommand(includeFile: false), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to update product", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenSucceedsWithFile_ReturnsMappedProductResponseWithNewImage()
    {
        var productRepo = new StubProductRepository { ProductById = CreateProduct() };
        var unitOfWork = new StubUnitOfWork(productRepo) { CompleteResult = 1 };
        var imageService = new StubImageService { UpdateResult = Result<string>.Success("images/updated.png") };
        var handler = new UpdateProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(MakeCommand(includeFile: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated Name", result.Value.Name);
        Assert.Equal("images/updated.png", result.Value.PictureUrl);
    }

    [Fact]
    public async Task Handle_WhenSucceedsWithoutFile_KeepsOriginalImage()
    {
        var productRepo = new StubProductRepository { ProductById = CreateProduct() };
        var unitOfWork = new StubUnitOfWork(productRepo) { CompleteResult = 1 };
        var imageService = new StubImageService();
        var handler = new UpdateProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(MakeCommand(includeFile: false), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("images/original.png", result.Value.PictureUrl);
    }

    private static Product CreateProduct() => new()
    {
        Id = 1,
        Name = "Old Name",
        Description = "Old Description",
        Price = 1000,
        PictureUrl = "images/original.png",
        Type = "Boots",
        Brand = "OldBrand",
        QuantityInStock = 5
    };

    private static UpdateProductCommand MakeCommand(bool includeFile) =>
        new(1, "Updated Name", "Updated Description", 3000, "Shoes", "NewBrand", 20,
            includeFile ? new StubFormFileService() : null!);

    private sealed class StubFormFileService : IFormFileService
    {
        public string FileName => "updated.png";
        public long Length => 200;
        public string ContentType => "image/png";
        public Stream OpenReadStream() => Stream.Null;
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public StubUnitOfWork(IProductRepository productRepository)
        {
            ProductRepository = productRepository;
        }

        public int CompleteResult { get; set; }
        public IBasketRepository BasketRepository { get; } = null!;
        public IOrderRepository OrderRepository { get; } = null!;
        public IProductRepository ProductRepository { get; }
        public IUserRepository UserRepository { get; } = null!;
        public Task<int> CompleteAsync() => Task.FromResult(CompleteResult);
        public void Dispose() { }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Product? ProductById { get; set; }

        public Task<Product?> GetByIdAsync(int id) => Task.FromResult(ProductById);
        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams) =>
            Task.FromResult(new PagedList<Product>(new List<Product>(), 0, 1, 6));
        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() => Task.FromResult((new List<string>(), new List<string>()));
        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;
        public Task DeleteAsync(Product product) => Task.CompletedTask;
    }

    private sealed class StubImageService : IImageService
    {
        public Result<string> UpdateResult { get; set; } = Result<string>.Success("images/updated.png");
        public Task<Result<string>> AddImageAsync(IFormFileService formFileService) => Task.FromResult(Result<string>.Failure("not used"));
        public Task<Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl) => Task.FromResult(UpdateResult);
        public Task<Result<bool>> DeleteImageAsync(string imagePath) => Task.FromResult(Result<bool>.Failure("not used"));
    }
}
