using AutoMapper;
using MediatR;
using Restore.Application.Queries;
using Restore.Application.Responses;
using Restore.Core.Repositories;

namespace Restore.Application.Handlers;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductByIdHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);
        var productResponse = _mapper.Map<ProductResponse>(product);
        return productResponse;
    }
}
