using Restore.Core.Entities;
using Restore.Core.Pagination;

namespace Restore.Core.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<PagedList<Product>> GetProductsAsync(ProductParams productParams);
    Task<Product?> GetByIdAsync(int id);
    Task<(List<string> Brands, List<string> Types)> GetProductsFilters();

    Task<Product?> ReadAsync(int productId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Product product);
}
