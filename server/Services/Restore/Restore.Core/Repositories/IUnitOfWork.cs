namespace Restore.Core.Repositories;

public interface IUnitOfWork : IDisposable
{
    IBasketRepository BasketRepository { get; }
    IOrderRepository OrderRepository { get; }
    IProductRepository ProductRepository { get; }
    IUserRepository UserRepository { get; }
    Task<int> CompleteAsync();
}
