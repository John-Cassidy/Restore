using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly StoreContext _context;
    private readonly IMapper _mapper;

    public OrderRepository(StoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<IReadOnlyList<Order>>> GetOrdersAsync(string buyerId)
    {
        var orders = await _context.Orders
            .Where(o => o.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered)
            .ToListAsync();

        return Result<IReadOnlyList<Order>>.Success(orders);
    }

    public async Task<Result<Order>> GetOrderByIdAsync(string buyerId, int orderId)
    {
        var order = await _context.Orders
            .Where(o => o.Id == orderId && o.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered)
            .FirstOrDefaultAsync();

        if (order == null)
        {
            return Result<Order>.Failure("Order not found");
        }

        return Result<Order>.Success(order);
    }

    public async Task<Order?> ReadAsync(string buyerId, int orderId)
    {
        return await _context.Orders
            .Where(o => o.Id == orderId && o.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .ThenInclude(i => i.ItemOrdered)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Order order)
    {
        await _context.Orders.AddAsync(order);
    }

    public async Task UpdateAsync(Order order)
    {
        var orderToUpdate = await _context.Orders.FindAsync(order.Id);

        if (orderToUpdate == null)
        {
            throw new Exception("Order not found");
        }

        _mapper.Map(order, orderToUpdate);
        _context.Orders.Update(orderToUpdate);
    }
}
