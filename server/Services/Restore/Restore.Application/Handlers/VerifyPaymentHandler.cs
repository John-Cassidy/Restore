using MediatR;
using Restore.Application.Commands;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Microsoft.Extensions.Configuration;
using Stripe;
using Restore.Core.Entities.OrderAggregate;

namespace Restore.Application.Handlers;

public class VerifyPaymentHandler : IRequestHandler<VerifyPaymentCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IConfiguration _config;

    public VerifyPaymentHandler(IUnitOfWork unitOfWork, IConfiguration config)
    {
        _unitOfWork = unitOfWork;
        _config = config;
    }

    public async Task<Result<Unit>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken)
    {
        var stripeEvent = EventUtility.ConstructEvent(
                request.StripeEvent,
                request.StripeSignature,
                _config["StripeSettings:WhSecret"]
            );

        var charge = (Charge)stripeEvent.Data.Object;

        var order = await _unitOfWork.OrderRepository.ReadOrderByPaymentIntentIdAsync(charge.PaymentIntentId);

        if (order is not null && charge.Status == "succeeded") order.OrderStatus = OrderStatus.PaymentReceived;

        await _unitOfWork.OrderRepository.UpdateAsync(order);

        var result = await _unitOfWork.CompleteAsync() > 0;

        if (result == false)
        {
            return Result<Unit>.Failure("Problem verifying payment");
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
