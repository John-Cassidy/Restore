using Microsoft.EntityFrameworkCore;
using Restore.Application.Extensions;
using Restore.Core.Entities;
using Restore.Core.Pagination;
using Restore.Core.Repositories;
using Restore.Infrastructure.Data;

namespace Restore.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly StoreContext _context;

    public ProductRepository(StoreContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public async Task<PagedList<Product>> GetProductsAsync(ProductParams productParams)
    {
        var query = _context.Products
            .Sort(productParams.OrderBy)
            .Search(productParams.SearchTerm)
            .Filter(productParams.Brands, productParams.Types)
            .AsQueryable();

        var count = await query.CountAsync();
        var items = await query.Skip((productParams.PageNumber - 1) * productParams.PageSize).Take(productParams.PageSize).ToListAsync();
        return new PagedList<Product>(items, count, productParams.PageNumber, productParams.PageSize);
    }
}
