using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Restore.Application.Services;
using Stripe;

namespace Restore.Infrastructure.Services;

[ExcludeFromCodeCoverage]
public class StripeEventParser : IStripeEventParser
{
    private readonly string _whSecret;

    public StripeEventParser(IConfiguration config)
    {
        _whSecret = config["StripeSettings:WhSecret"]
            ?? throw new InvalidOperationException("StripeSettings:WhSecret is not configured");
    }

    public Event ParseEvent(string json, string signature)
    {
        return EventUtility.ConstructEvent(json, signature, _whSecret);
    }
}
