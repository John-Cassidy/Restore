using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Application.Services;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class DeleteProductHandlerTests
{
    [Fact]
    public async Task Handle_WhenProductIsMissing_ReturnsFailure()
    {
        var productRepository = new StubProductRepository { ProductById = null };
        var unitOfWork = new StubUnitOfWork(productRepository) { CompleteResult = 1 };
        var imageService = new StubImageService { DeleteResult = Result<bool>.Success(true) };
        var handler = new DeleteProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(new DeleteProductCommand(9), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product not found", result.ErrorMessage);
        Assert.False(productRepository.DeleteCalled);
    }

    [Fact]
    public async Task Handle_WhenImageDeleteFails_ReturnsFailure()
    {
        var productRepository = new StubProductRepository
        {
            ProductById = CreateProduct("images/product.png")
        };
        var unitOfWork = new StubUnitOfWork(productRepository) { CompleteResult = 1 };
        var imageService = new StubImageService { DeleteResult = Result<bool>.Failure("image delete failed") };
        var handler = new DeleteProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(new DeleteProductCommand(9), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("image delete failed", result.ErrorMessage);
        Assert.False(productRepository.DeleteCalled);
    }

    [Fact]
    public async Task Handle_WhenCommitFails_ReturnsFailure()
    {
        var productRepository = new StubProductRepository
        {
            ProductById = CreateProduct("images/product.png")
        };
        var unitOfWork = new StubUnitOfWork(productRepository) { CompleteResult = 0 };
        var imageService = new StubImageService { DeleteResult = Result<bool>.Success(true) };
        var handler = new DeleteProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(new DeleteProductCommand(9), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Failed to delete product", result.ErrorMessage);
        Assert.True(productRepository.DeleteCalled);
    }

    [Fact]
    public async Task Handle_WhenAllOperationsSucceed_ReturnsSuccess()
    {
        var productRepository = new StubProductRepository
        {
            ProductById = CreateProduct("images/product.png")
        };
        var unitOfWork = new StubUnitOfWork(productRepository) { CompleteResult = 1 };
        var imageService = new StubImageService { DeleteResult = Result<bool>.Success(true) };
        var handler = new DeleteProductHandler(unitOfWork, imageService);

        var result = await handler.Handle(new DeleteProductCommand(9), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.True(productRepository.DeleteCalled);
        Assert.Equal("images/product.png", imageService.LastDeletedPath);
    }

    private static Product CreateProduct(string? pictureUrl) => new()
    {
        Id = 9,
        Name = "Boots",
        Description = "All weather",
        Price = 5000,
        PictureUrl = pictureUrl!,
        Type = "Shoes",
        Brand = "Restore",
        QuantityInStock = 3
    };

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public StubUnitOfWork(IProductRepository productRepository)
        {
            ProductRepository = productRepository;
            BasketRepository = new StubBasketRepository();
            OrderRepository = new StubOrderRepository();
            UserRepository = new StubUserRepository();
        }

        public IBasketRepository BasketRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IUserRepository UserRepository { get; }
        public int CompleteResult { get; set; }

        public Task<int> CompleteAsync() => Task.FromResult(CompleteResult);
        public void Dispose() { }
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Product? ProductById { get; set; }
        public bool DeleteCalled { get; private set; }

        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
        public Task<Restore.Core.Pagination.PagedList<Product>> GetProductsAsync(Restore.Core.Pagination.ProductParams productParams) =>
            Task.FromResult(new Restore.Core.Pagination.PagedList<Product>(new List<Product>(), 0, 1, 6));
        public Task<Product?> GetByIdAsync(int id) => Task.FromResult(ProductById);
        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() => Task.FromResult((new List<string>(), new List<string>()));
        public Task<Product?> ReadAsync(int productId) => Task.FromResult<Product?>(null);
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;

        public Task DeleteAsync(Product product)
        {
            DeleteCalled = true;
            return Task.CompletedTask;
        }
    }

    private sealed class StubImageService : IImageService
    {
        public Result<bool> DeleteResult { get; set; } = Result<bool>.Success(true);
        public string? LastDeletedPath { get; private set; }

        public Task<Result<string>> AddImageAsync(IFormFileService formFileService) => Task.FromResult(Result<string>.Failure("not used"));
        public Task<Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl) => Task.FromResult(Result<string>.Failure("not used"));

        public Task<Result<bool>> DeleteImageAsync(string imagePath)
        {
            LastDeletedPath = imagePath;
            return Task.FromResult(DeleteResult);
        }
    }

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult<Basket?>(null);
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;
        public Task DeleteAsync(Basket basket) => Task.CompletedTask;
    }

    private sealed class StubOrderRepository : IOrderRepository
    {
        public Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId) => Task.FromResult(Result<IReadOnlyList<Order>>.Failure("not used"));
        public Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId) => Task.FromResult(Result<Order>.Failure("not used"));
        public Task<Order?> ReadOrderByPaymentIntentIdAsync(string paymentIntentId) => Task.FromResult<Order?>(null);
        public Task<Order?> ReadAsync(string buyerId, int orderId) => Task.FromResult<Order?>(null);
        public Task AddAsync(Order order) => Task.CompletedTask;
        public Task UpdateAsync(Order order) => Task.CompletedTask;
    }

    private sealed class StubUserRepository : IUserRepository
    {
        public Task<Result<User>> GetUserAsync(string username) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> LoginAsync(string username, string password) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> RegisterAsync(string username, string password, string email) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task<User?> ReadUserAddressAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}