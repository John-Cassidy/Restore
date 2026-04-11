using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Application.Requests;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class CreateOrderHandlerTests
{
    private static AddressRequest DefaultAddress => new(
        "John Doe", "123 Main St", "Apt 4", "Portland", "OR", "97201", "US");

    private static CreateOrderCommand MakeCommand(bool saveAddress = false) =>
        new("buyer1", "john", new CreateOrderCommandRequest(saveAddress, DefaultAddress));

    private static Basket MakeBasket(params (int productId, int quantity)[] items)
    {
        var basket = new Basket { BuyerId = "buyer1", PaymentIntentId = "pi_123" };
        foreach (var (productId, quantity) in items)
        {
            basket.Items.Add(new BasketItem { ProductId = productId, Quantity = quantity });
        }
        return basket;
    }

    private static Product MakeProduct(int id, long price, int stock = 100) => new()
    {
        Id = id,
        Name = $"Product {id}",
        Description = "Test product",
        Price = price,
        PictureUrl = $"images/product{id}.png",
        Type = "Shoes",
        Brand = "Restore",
        QuantityInStock = stock
    };

    [Fact]
    public async Task Handle_WhenBasketIsNull_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = null };
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo);
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Could not locate basket", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenBasketIsEmpty_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = new Basket { BuyerId = "buyer1" } };
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo);
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Could not locate basket", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenProductNotFound_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((99, 2)) };
        var productRepo = new StubProductRepository(); // No products configured
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo);
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Product not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenSaveAddressAndUserNotFound_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var productRepo = new StubProductRepository();
        productRepo.Products[1] = MakeProduct(1, 5000);
        var userRepo = new StubUserRepository { UserResult = null };
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo, userRepo: userRepo)
        {
            CompleteResult = 1
        };
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(saveAddress: true), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("User not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenDbSaveFails_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var productRepo = new StubProductRepository();
        productRepo.Products[1] = MakeProduct(1, 5000);
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo) { CompleteResult = 0 };
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Problem saving changes", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenSubtotalAboveThreshold_DeliveryFeeIsZero()
    {
        // Price 6000 * quantity 2 = 12000 > 10000 threshold → free delivery
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var productRepo = new StubProductRepository();
        productRepo.Products[1] = MakeProduct(1, 6000);
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo) { CompleteResult = 1 };
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(12000, result.Value.Subtotal);
        Assert.Equal(0, result.Value.DeliveryFee);
        Assert.Equal(12000, result.Value.Total);
    }

    [Fact]
    public async Task Handle_WhenSubtotalAtOrBelowThreshold_DeliveryFeeIs500()
    {
        // Price 5000 * quantity 2 = 10000 ≤ 10000 threshold → 500 delivery fee
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var productRepo = new StubProductRepository();
        productRepo.Products[1] = MakeProduct(1, 5000);
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo) { CompleteResult = 1 };
        var handler = new CreateOrderHandler(unitOfWork);

        var result = await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(10000, result.Value.Subtotal);
        Assert.Equal(500, result.Value.DeliveryFee);
        Assert.Equal(10500, result.Value.Total);
        Assert.True(basketRepo.DeleteCalled);
    }

    [Fact]
    public async Task Handle_DeductsStockFromProduct()
    {
        var product = MakeProduct(1, 5000, stock: 10);
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 3)) };
        var productRepo = new StubProductRepository();
        productRepo.Products[1] = product;
        var unitOfWork = new StubUnitOfWork(basketRepo: basketRepo, productRepo: productRepo) { CompleteResult = 1 };
        var handler = new CreateOrderHandler(unitOfWork);

        await handler.Handle(MakeCommand(), CancellationToken.None);

        Assert.Equal(7, product.QuantityInStock); // 10 - 3
    }

    // --- Stubs ---

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public StubUnitOfWork(
            StubBasketRepository? basketRepo = null,
            StubProductRepository? productRepo = null,
            StubUserRepository? userRepo = null)
        {
            BasketRepository = basketRepo ?? new StubBasketRepository();
            ProductRepository = productRepo ?? new StubProductRepository();
            OrderRepository = new StubOrderRepository();
            UserRepository = userRepo ?? new StubUserRepository();
        }

        public IBasketRepository BasketRepository { get; }
        public IOrderRepository OrderRepository { get; }
        public IProductRepository ProductRepository { get; }
        public IUserRepository UserRepository { get; }
        public int CompleteResult { get; set; }

        public Task<int> CompleteAsync() => Task.FromResult(CompleteResult);
        public void Dispose() { }
    }

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Basket? BasketResult { get; set; }
        public bool DeleteCalled { get; private set; }

        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult(BasketResult);

        public Task DeleteAsync(Basket basket)
        {
            DeleteCalled = true;
            return Task.CompletedTask;
        }

        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;
    }

    private sealed class StubProductRepository : IProductRepository
    {
        public Dictionary<int, Product> Products { get; } = new();

        public Task<Product?> ReadAsync(int productId) =>
            Task.FromResult(Products.GetValueOrDefault(productId));

        public Task<IReadOnlyList<Product>> GetProductsAsync() => Task.FromResult<IReadOnlyList<Product>>(new List<Product>());
        public Task<PagedList<Product>> GetProductsAsync(ProductParams productParams) =>
            Task.FromResult(new PagedList<Product>(new List<Product>(), 0, 1, 6));
        public Task<Product?> GetByIdAsync(int id) => Task.FromResult<Product?>(null);
        public Task<(List<string> Brands, List<string> Types)> GetProductsFilters() => Task.FromResult((new List<string>(), new List<string>()));
        public Task AddAsync(Product product) => Task.CompletedTask;
        public Task UpdateAsync(Product product) => Task.CompletedTask;
        public Task DeleteAsync(Product product) => Task.CompletedTask;
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
        public User? UserResult { get; set; }

        public Task<User?> ReadUserAddressAsync(string username) => Task.FromResult(UserResult);

        public Task<Result<User>> GetUserAsync(string username) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> LoginAsync(string username, string password) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<Result<User>> RegisterAsync(string username, string password, string email) => Task.FromResult(Result<User>.Failure("not used"));
        public Task<User?> ReadAsync(string username) => Task.FromResult<User?>(null);
        public Task UpdateAsync(User user) => Task.CompletedTask;
    }
}
