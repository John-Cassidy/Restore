using Restore.Core.Entities;
using Stripe;

namespace Restore.Application.Services;

public interface IPaymentService
{
    Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket);
}
