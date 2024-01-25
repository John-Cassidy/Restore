using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly StoreContext _context;

    public BasketRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<Result<Basket>> GetBasketAsync(string buyerId)
    {
        var result = await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(x => x.BuyerId == buyerId) ?? new Basket { BuyerId = buyerId };

        return Result<Basket>.Success(result);
    }

    public async Task<Result<Basket>> AddItemToBasketAsync(string buyerId, int productId, int quantity)
    {
        var basket = await _context.Baskets
        .Include(b => b.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

        if (basket == null)
        {
            basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
        }

        var product = await _context.Products.FindAsync(productId);

        if (product == null) return Result<Basket>.Failure("Product not found");

        basket.AddItem(product, quantity);

        var result = await _context.SaveChangesAsync() > 0;

        return result ? Result<Basket>.Success(basket) : Result<Basket>.Failure("Problem saving item to basket");
    }

    public async Task<Result<Basket>> RemoveItemFromBasketAsync(string buyerId, int productId, int quantity)
    {
        var basket = await _context.Baskets
        .Include(b => b.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

        if (basket == null) return Result<Basket>.Failure("Basket not found");

        var product = await _context.Products.FindAsync(productId);

        if (product == null) return Result<Basket>.Failure("Product not found");

        basket.RemoveItem(productId, quantity);

        var result = await _context.SaveChangesAsync() > 0;

        return result ? Result<Basket>.Success(basket) : Result<Basket>.Failure("Problem removing item from basket");
    }
}
