using MediatR;
using Restore.Application.Responses;
using Restore.Core.Pagination;

namespace Restore.Application.Queries;

public class GetProductsQuery : IRequest<PagedList<ProductResponse>>
{
    public ProductParams ProductParams { get; set; }
    public GetProductsQuery(ProductParams productParams)
    {
        ProductParams = productParams;
    }
}
