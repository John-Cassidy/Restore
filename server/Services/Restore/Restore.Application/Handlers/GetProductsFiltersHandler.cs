using AutoMapper;
using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductsFiltersHandler : IRequestHandler<GetProductsFiltersQuery, ProductsFiltersResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsFiltersHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductsFiltersResponse> Handle(GetProductsFiltersQuery request, CancellationToken cancellationToken)
    {
        var filters = await _productRepository.GetProductsFilters();
        var response = new ProductsFiltersResponse(filters.Brands, filters.Types);
        return response;
    }
}
