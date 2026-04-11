using MediatR;
using Restore.Application.Commands;
using Restore.Application.Services;
using Restore.Core.Entities.OrderAggregate;
using Restore.Core.Repositories;
using Restore.Core.Results;
using Stripe;

namespace Restore.Application.Handlers;

public class VerifyPaymentHandler : IRequestHandler<VerifyPaymentCommand, Result<Unit>> {
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripeEventParser _stripeEventParser;

    public VerifyPaymentHandler(IUnitOfWork unitOfWork, IStripeEventParser stripeEventParser) {
        _unitOfWork = unitOfWork;
        _stripeEventParser = stripeEventParser;
    }

    public async Task<Result<Unit>> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken) {
        var stripeEvent = _stripeEventParser.ParseEvent(
                request.StripeEvent,
                request.StripeSignature
            );

        var charge = (Charge)stripeEvent.Data.Object;

        var order = await _unitOfWork.OrderRepository.ReadOrderByPaymentIntentIdAsync(charge.PaymentIntentId);

        if (order is not null && charge.Status == "succeeded")
            order.OrderStatus = OrderStatus.PaymentReceived;

        await _unitOfWork.OrderRepository.UpdateAsync(order);

        var result = await _unitOfWork.CompleteAsync() > 0;

        if (result == false) {
            return Result<Unit>.Failure("Problem verifying payment");
        }

        return Result<Unit>.Success(Unit.Value);
    }
}
