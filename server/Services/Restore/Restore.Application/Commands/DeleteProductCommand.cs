using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class DeleteProductCommand : IRequest<Result<bool>>
{
    public DeleteProductCommand(int id)
    {
        Id = id;
    }

    public int Id { get; }
}