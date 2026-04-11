using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductsFiltersHandler : IRequestHandler<GetProductsFiltersQuery, ProductsFiltersResponse> {
    private readonly IProductRepository _productRepository;

    public GetProductsFiltersHandler(IProductRepository productRepository) {
        _productRepository = productRepository;
    }

    public async Task<ProductsFiltersResponse> Handle(GetProductsFiltersQuery request, CancellationToken cancellationToken) {
        var filters = await _productRepository.GetProductsFilters();
        var response = new ProductsFiltersResponse(filters.Brands, filters.Types);
        return response;
    }
}
