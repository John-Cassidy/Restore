using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class GetOrderByIdHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryFails_ReturnsFailure()
    {
        var repository = new StubOrderRepository
        {
            OrderResult = Result<Order>.Failure("missing")
        };
        var handler = new GetOrderByIdHandler(repository);

        var result = await handler.Handle(new GetOrderByIdQuery("buyer-1", 99), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Order not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenRepositorySucceeds_ReturnsMappedOrder()
    {
        var repository = new StubOrderRepository
        {
            OrderResult = Result<Order>.Success(CreateOrder(99))
        };
        var handler = new GetOrderByIdHandler(repository);

        var result = await handler.Handle(new GetOrderByIdQuery("buyer-1", 99), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(99, result.Value.Id);
        Assert.Equal(5500, result.Value.Total);
    }

    private static Order CreateOrder(int id) => new()
    {
        Id = id,
        BuyerId = "buyer-1",
        ShippingAddress = new ShippingAddress
        {
            FullName = "John",
            Address1 = "1 Main",
            Address2 = "Apt 2",
            City = "NYC",
            State = "NY",
            Zip = "10001",
            Country = "US"
        },
        Subtotal = 5000,
        DeliveryFee = 500,
        PaymentIntentId = "pi_123",
        OrderItems = new List<OrderItem> {
            new() {
                Id = 1,
                Price = 5000,
                Quantity = 1,
                ItemOrdered = new ProductItemOrdered {
                    ProductId = 5,
                    Name = "Boots",
                    PictureUrl = "images/boots.png"
                }
            }
        }
    };

    private sealed class StubOrderRepository : IOrderRepository
    {
        public Result<Order> OrderResult { get; set; } = Result<Order>.Failure("not used");

        public Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId) => Task.FromResult(Result<IReadOnlyList<Order>>.Failure("not used"));
        public Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId) => Task.FromResult(OrderResult);
        public Task<Order?> ReadOrderByPaymentIntentIdAsync(string paymentIntentId) => Task.FromResult<Order?>(null);
        public Task<Order?> ReadAsync(string buyerId, int orderId) => Task.FromResult<Order?>(null);
        public Task AddAsync(Order order) => Task.CompletedTask;
        public Task UpdateAsync(Order order) => Task.CompletedTask;
    }
}