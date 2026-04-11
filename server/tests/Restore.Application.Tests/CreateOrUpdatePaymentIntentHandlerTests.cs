using Restore.Application.Commands;
using Restore.Application.Handlers;
using Restore.Application.Services;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Stripe;
using Product = Restore.Core.Entities.Product;

namespace Restore.Application.Tests;

public class CreateOrUpdatePaymentIntentHandlerTests
{
    private static Basket MakeBasket(params (int productId, int quantity)[] items)
    {
        var basket = new Basket { BuyerId = "buyer1" };
        foreach (var (productId, quantity) in items)
        {
            basket.Items.Add(new BasketItem
            {
                ProductId = productId,
                Quantity = quantity,
                Product = new Product
                {
                    Id = productId,
                    Name = $"Product {productId}",
                    Description = "Test",
                    Price = 1000,
                    PictureUrl = "img.png",
                    Type = "Test",
                    Brand = "Test",
                    QuantityInStock = 10
                }
            });
        }
        return basket;
    }

    [Fact]
    public async Task Handle_WhenBasketIsNull_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = null };
        var unitOfWork = new StubUnitOfWork(basketRepo);
        var paymentService = new StubPaymentService();
        var handler = new CreateOrUpdatePaymentIntentHandler(unitOfWork, paymentService);

        var result = await handler.Handle(
            new CreateOrUpdatePaymentIntentCommand("buyer1"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenPaymentIntentIsNull_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var unitOfWork = new StubUnitOfWork(basketRepo);
        var paymentService = new StubPaymentService { Intent = null };
        var handler = new CreateOrUpdatePaymentIntentHandler(unitOfWork, paymentService);

        var result = await handler.Handle(
            new CreateOrUpdatePaymentIntentCommand("buyer1"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Problem creating payment intent", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenDbSaveFails_ReturnsFailure()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var unitOfWork = new StubUnitOfWork(basketRepo) { CompleteResult = 0 };
        var paymentService = new StubPaymentService
        {
            Intent = new PaymentIntent { Id = "pi_123", ClientSecret = "sec_abc" }
        };
        var handler = new CreateOrUpdatePaymentIntentHandler(unitOfWork, paymentService);

        var result = await handler.Handle(
            new CreateOrUpdatePaymentIntentCommand("buyer1"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Contains("Problem updating basket with intent", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ReturnsBasketResponseWithIntentFields()
    {
        var basketRepo = new StubBasketRepository { BasketResult = MakeBasket((1, 2)) };
        var unitOfWork = new StubUnitOfWork(basketRepo) { CompleteResult = 1 };
        var paymentService = new StubPaymentService
        {
            Intent = new PaymentIntent { Id = "pi_123", ClientSecret = "sec_abc" }
        };
        var handler = new CreateOrUpdatePaymentIntentHandler(unitOfWork, paymentService);

        var result = await handler.Handle(
            new CreateOrUpdatePaymentIntentCommand("buyer1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("pi_123", result.Value.PaymentIntentId);
        Assert.Equal("sec_abc", result.Value.ClientSecret);
    }

    // --- Stubs ---

    private sealed class StubPaymentService : IPaymentService
    {
        public PaymentIntent? Intent { get; set; }

        public Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket) =>
            Task.FromResult(Intent!);
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public StubUnitOfWork(StubBasketRepository? basketRepo = null)
        {
            BasketRepository = basketRepo ?? new StubBasketRepository();
            ProductRepository = new StubProductRepository();
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

    private sealed class StubBasketRepository : IBasketRepository
    {
        public Basket? BasketResult { get; set; }

        public Task<Basket?> ReadAsync(string buyerId) => Task.FromResult(BasketResult);
        public Task UpdateAsync(Basket basket) => Task.CompletedTask;

        public Task<Result<Basket>> GetBasketAsync(string buyerId) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity) => Task.FromResult(Result<Basket>.Failure("not used"));
        public Task<Result<bool>> DeleteBasketAsync(string buyerId) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task<Result<bool>> UpdateBasketAsync(string buyerId, string username) => Task.FromResult(Result<bool>.Failure("not used"));
        public Task AddAsync(Basket basket) => Task.CompletedTask;
        public Task DeleteAsync(Basket basket) => Task.CompletedTask;
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
