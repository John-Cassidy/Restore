using Stripe;

namespace Restore.Application.Services;

public interface IStripeEventParser
{
    Event ParseEvent(string json, string signature);
}
