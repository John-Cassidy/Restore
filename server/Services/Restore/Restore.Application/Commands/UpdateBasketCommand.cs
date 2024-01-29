using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class UpdateBasketCommand : IRequest<Result<bool>>
{
    public UpdateBasketCommand(string buyerId, string username)
    {
        BuyerId = buyerId;
        Username = username;
    }

    public string BuyerId { get; }
    public string Username { get; }
}
