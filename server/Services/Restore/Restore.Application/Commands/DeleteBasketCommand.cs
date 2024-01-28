using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class DeleteBasketCommand : IRequest<Result<bool>>
{
    public DeleteBasketCommand(string buyerId)
    {
        BuyerId = buyerId;
    }

    public string BuyerId { get; }
}
