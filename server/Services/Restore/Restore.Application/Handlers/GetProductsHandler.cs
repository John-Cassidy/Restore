using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Pagination;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, PagedList<ProductResponse>>
{
    private readonly IProductRepository _productRepository;
    public GetProductsHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedList<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await _productRepository.GetProductsAsync(request.ProductParams);
        var productResponseList = ProductMapper.Mapper.Map<PagedList<ProductResponse>>(productList);
        return productResponseList;
    }
}
