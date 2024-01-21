using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Entities;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);

        var productResponse = ProductMapper.Mapper.Map<ProductResponse>(product);

        return productResponse;
    }
}
