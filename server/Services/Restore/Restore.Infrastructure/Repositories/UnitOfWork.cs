using Restore.Core.Repositories;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly StoreContext _context;
    public IBasketRepository BasketRepository { get; }
    public IOrderRepository OrderRepository { get; }
    public IProductRepository ProductRepository { get; }
    public IUserRepository UserRepository { get; }

    public UnitOfWork(StoreContext context, IBasketRepository basketRepository, IOrderRepository orderRepository, IProductRepository productRepository, IUserRepository userRepository)
    {
        _context = context;
        BasketRepository = basketRepository;
        OrderRepository = orderRepository;
        ProductRepository = productRepository;
        UserRepository = userRepository;
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
