using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restore.Application.Services;
using Restore.Core.Entities;
using Stripe;

namespace Restore.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _config;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(IConfiguration config, ILogger<PaymentService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket)
    {
        _logger.LogInformation("Creating or updating payment intent for basket id {BasketId}", basket.Id);

        StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

        var service = new PaymentIntentService();
        PaymentIntent intent = new PaymentIntent();

        var subtotal = basket.Items.Sum(i => i.Quantity * i.Product.Price);
        var deliveryFee = subtotal > 10000 ? 0 : 500;

        if (string.IsNullOrEmpty(basket.PaymentIntentId))
        {
            _logger.LogInformation("No existing payment intent found. Creating a new one.");

            var options = new PaymentIntentCreateOptions
            {
                Amount = subtotal + deliveryFee,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" }
            };
            intent = await service.CreateAsync(options);
            basket.PaymentIntentId = intent.Id;
            basket.ClientSecret = intent.ClientSecret;

            _logger.LogInformation("Created new payment intent with id {PaymentIntentId}", intent.Id);
        }
        else
        {
            _logger.LogInformation("Existing payment intent found. Updating it.");

            var options = new PaymentIntentUpdateOptions
            {
                Amount = subtotal + deliveryFee
            };
            intent = await service.UpdateAsync(basket.PaymentIntentId, options);

            _logger.LogInformation("Updated payment intent with id {PaymentIntentId}", intent.Id);
        }

        return intent;
    }
}
