using AutoMapper;
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
    private readonly IMapper _mapper;

    public GetProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<PagedList<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var productList = await _productRepository.GetProductsAsync(request.ProductParams);
        var productResponseList = _mapper.Map<PagedList<ProductResponse>>(productList);
        return productResponseList;
    }
}
