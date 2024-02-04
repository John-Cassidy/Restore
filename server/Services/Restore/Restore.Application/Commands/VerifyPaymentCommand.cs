using MediatR;
using Restore.Core.Results;

namespace Restore.Application.Commands;

public class VerifyPaymentCommand : IRequest<Result<Unit>>
{
    public string StripeEvent { get; set; }
    public string StripeSignature { get; set; }

    public VerifyPaymentCommand(string stripeEvent, string stripeSignature)
    {
        StripeEvent = stripeEvent;
        StripeSignature = stripeSignature;
    }
}
