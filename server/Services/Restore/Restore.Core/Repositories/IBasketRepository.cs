using Restore.Core.Entities;
using Restore.Core.Results;

namespace Restore.Core.Repositories;

public interface IBasketRepository
{
    Task<Result<Basket>> GetBasketAsync(string buyerId);
    Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity);
    Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity);
    Task<Result<bool>> DeleteBasketAsync(string buyerId);
    Task<Result<bool>> UpdateBasketAsync(string buyerId, string username);
}
