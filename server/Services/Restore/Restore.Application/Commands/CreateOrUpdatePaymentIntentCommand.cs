using MediatR;
using Restore.Application.Responses;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class CreateOrUpdatePaymentIntentCommand : IRequest<Result<BasketResponse>>
{
    public string BuyerId { get; }

    public CreateOrUpdatePaymentIntentCommand(string buyerId)
    {
        BuyerId = buyerId;
    }
}