using MediatR;
using Restore.Application.Mappers;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse> {
    private readonly IProductRepository _productRepository;

    public GetProductByIdHandler(IProductRepository productRepository) {
        _productRepository = productRepository;
    }

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken) {
        var product = await _productRepository.GetByIdAsync(request.Id);
        return product.ToProductResponse();
    }
}
