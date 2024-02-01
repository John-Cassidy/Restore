using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Results;

namespace Restore.Core.Repositories;

public interface IOrderRepository
{
    Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId);
    Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId);

    Task<Order?> ReadAsync(string buyerId, int orderId);
    Task AddAsync(Order order);
    Task UpdateAsync(Order order);
}
