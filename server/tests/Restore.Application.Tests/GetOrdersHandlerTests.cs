using Restore.Application.Handlers;
using Restore.Application.Queries;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;

namespace Restore.Application.Tests;

public class GetOrdersHandlerTests
{
    [Fact]
    public async Task Handle_WhenRepositoryFails_ReturnsFailure()
    {
        var repository = new StubOrderRepository
        {
            OrdersResult = Result<IReadOnlyList<Order>>.Failure("missing")
        };
        var handler = new GetOrdersHandler(repository);

        var result = await handler.Handle(new GetOrdersQuery("buyer-1"), CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Orders not found", result.ErrorMessage);
    }

    [Fact]
    public async Task Handle_WhenRepositorySucceeds_ReturnsMappedOrders()
    {
        var repository = new StubOrderRepository
        {
            OrdersResult = Result<IReadOnlyList<Order>>.Success(new List<Order> { CreateOrder(10) })
        };
        var handler = new GetOrdersHandler(repository);

        var result = await handler.Handle(new GetOrdersQuery("buyer-1"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value);
        Assert.Equal(10, result.Value[0].Id);
        Assert.Equal(5500, result.Value[0].Total);
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
        public Result<IReadOnlyList<Order>> OrdersResult { get; set; } = Result<IReadOnlyList<Order>>.Failure("not used");

        public Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId) => Task.FromResult(OrdersResult);
        public Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId) => Task.FromResult(Result<Order>.Failure("not used"));
        public Task<Order?> ReadOrderByPaymentIntentIdAsync(string paymentIntentId) => Task.FromResult<Order?>(null);
        public Task<Order?> ReadAsync(string buyerId, int orderId) => Task.FromResult<Order?>(null);
        public Task AddAsync(Order order) => Task.CompletedTask;
        public Task UpdateAsync(Order order) => Task.CompletedTask;
    }
}