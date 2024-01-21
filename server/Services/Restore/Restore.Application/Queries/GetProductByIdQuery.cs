using MediatR;
using Restore.Application.Responses;

namespace Restore.Application.Queries;

public class GetProductByIdQuery : IRequest<ProductResponse>
{
    public GetProductByIdQuery(int id)
    {
        Id = id;
    }

    public int Id { get; }
}
