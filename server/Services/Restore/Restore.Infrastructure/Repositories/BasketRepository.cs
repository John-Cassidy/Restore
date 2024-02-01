using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class BasketRepository : IBasketRepository
{
    private readonly StoreContext _context;
    private readonly IMapper _mapper;

    public BasketRepository(StoreContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

    public async Task<Result<bool>> UpdateBasketAsync(string buyerId, string username)
    {
        var basket = await _context.Baskets
        .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

        if (basket == null) return Result<bool>.Failure("Basket not found");

        basket.BuyerId = username;

        var result = await _context.SaveChangesAsync() > 0;

        return result ? Result<bool>.Success(true) : Result<bool>.Failure("Problem updating basket");
    }

    public async Task<Result<bool>> DeleteBasketAsync(string buyerId)
    {

        var basket = await _context.Baskets
        .FirstOrDefaultAsync(b => b.BuyerId == buyerId);

        if (basket == null) return Result<bool>.Success(true);

        _context.Baskets.Remove(basket);
        var result = await _context.SaveChangesAsync() > 0;
        return result ? Result<bool>.Success(true) : Result<bool>.Failure("Problem deleting basket");
    }

    public async Task<Basket?> ReadAsync(string buyerId)
    {
        return await _context.Baskets
            .Include(b => b.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(b => b.BuyerId == buyerId);
    }

    public async Task AddAsync(Basket basket)
    {
        await _context.Baskets.AddAsync(basket);
    }

    public async Task UpdateAsync(Basket basket)
    {
        var basketToUpdate = await _context.Baskets.FindAsync(basket.Id);

        if (basketToUpdate == null)
        {
            throw new Exception("Basket not found");
        }

        _mapper.Map(basket, basketToUpdate);
        _context.Baskets.Update(basketToUpdate);
    }

    public Task DeleteAsync(Basket basket)
    {
        _context.Baskets.Remove(basket);
        return Task.CompletedTask;
    }
}
