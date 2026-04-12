using MediatR;
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

public class VerifyPaymentHandlerTests
{
    private static Event MakeEvent(string paymentIntentId, string chargeStatus)
    {
        return new Event
        {
            Data = new EventData
            {
                Object = new Charge
                {
                    PaymentIntentId = paymentIntentId,
                    Status = chargeStatus
                }
            }
        };
    }

    [Fact]
    public async Task Handle_WhenOrderExistsAndChargeSucceeded_SetsPaymentReceived()
    {
        var order = new Order
        {
            Id = 1,
            BuyerId = "buyer1",
            PaymentIntentId = "pi_123",
            OrderStatus = OrderStatus.Pending
        };
        var orderRepo = new StubOrderRepository { OrderByPaymentIntentId = order };
        var unitOfWork = new StubUnitOfWork(orderRepo: orderRepo) { CompleteResult = 1 };
        var parser = new StubStripeEventParser
        {
            EventResult = MakeEvent("pi_123", "succeeded")
        };
        var handler = new VerifyPaymentHandler(unitOfWork, parser);

        var result = await handler.Handle(
            new VerifyPaymentCommand("json", "sig"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.PaymentReceived, order.OrderStatus);
    }

    [Fact]
    public async Task Handle_WhenChargeNotSucceeded_OrderStatusUnchanged()
    {
        var order = new Order
        {
            Id = 1,
            BuyerId = "buyer1",
            PaymentIntentId = "pi_123",
            OrderStatus = OrderStatus.Pending
        };
        var orderRepo = new StubOrderRepository { OrderByPaymentIntentId = order };
        var unitOfWork = new StubUnitOfWork(orderRepo: orderRepo) { CompleteResult = 1 };
        var parser = new StubStripeEventParser
        {
            EventResult = MakeEvent("pi_123", "failed")
        };
        var handler = new VerifyPaymentHandler(unitOfWork, parser);

        var result = await handler.Handle(
            new VerifyPaymentCommand("json", "sig"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(OrderStatus.Pending, order.OrderStatus);
    }

    [Fact]
    public async Task Handle_WhenDbSaveFails_ReturnsFailure()
    {
        var orderRepo = new StubOrderRepository { OrderByPaymentIntentId = null };
        var unitOfWork = new StubUnitOfWork(orderRepo: orderRepo) { CompleteResult = 0 };
        var parser = new StubStripeEventParser
        {
            EventResult = MakeEvent("pi_123", "succeeded")
        };
        var handler = new VerifyPaymentHandler(unitOfWork, parser);

        var result = await handler.Handle(
            new VerifyPaymentCommand("json", "sig"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Problem verifying payment", result.ErrorMessage);
    }

    // --- Stubs ---

    private sealed class StubStripeEventParser : IStripeEventParser
    {
        public Event EventResult { get; set; } = new();

        public Event ParseEvent(string json, string signature) => EventResult;
    }

    private sealed class StubUnitOfWork : IUnitOfWork
    {
        public StubUnitOfWork(StubOrderRepository? orderRepo = null)
        {
            BasketRepository = new StubBasketRepository();
            ProductRepository = new StubProductRepository();
            OrderRepository = orderRepo ?? new StubOrderRepository();
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

    private sealed class StubOrderRepository : IOrderRepository
    {
        public Order? OrderByPaymentIntentId { get; set; }
        public bool UpdateCalled { get; private set; }

        public Task<Order?> ReadOrderByPaymentIntentIdAsync(string paymentIntentId) =>
            Task.FromResult(OrderByPaymentIntentId);

        public Task UpdateAsync(Order order)
        {
            UpdateCalled = true;
            return Task.CompletedTask;
        }

        public Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId) => Task.FromResult(Result<IReadOnlyList<Order>>.Failure("not used"));
        public Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId) => Task.FromResult(Result<Order>.Failure("not used"));
        public Task<Order?> ReadAsync(string buyerId, int orderId) => Task.FromResult<Order?>(null);
        public Task AddAsync(Order order) => Task.CompletedTask;
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
