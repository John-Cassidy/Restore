using Restore.Core.Entities;
using Restore.Core.Pagination;

namespace Restore.Core.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetProductsAsync();
    Task<PagedList<Product>> GetProductsAsync(ProductParams productParams);
    Task<Product?> GetByIdAsync(int id);

    // Task<IReadOnlyList<ProductBrand>> GetProductBrandsAsync();
    // Task<IReadOnlyList<ProductType>> GetProductTypesAsync();
}
