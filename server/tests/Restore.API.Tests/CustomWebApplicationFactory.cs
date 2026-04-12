using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Restore.API.Tests.Fixtures;
using Restore.Application.Services;
using Restore.Core.Entities;
using Stripe;

namespace Restore.API.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly PostgresContainerFixture _postgres;

    public CustomWebApplicationFactory(PostgresContainerFixture postgres)
    {
        _postgres = postgres;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureServices(services =>
        {
            // Replace the real DbContext connection string with the test container's
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<Restore.Infrastructure.Data.StoreContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<Restore.Infrastructure.Data.StoreContext>(options =>
                options.UseNpgsql(_postgres.ConnectionString));

            // Replace IPaymentService with a stub so tests don't hit Stripe
            var paymentDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IPaymentService));
            if (paymentDescriptor is not null)
                services.Remove(paymentDescriptor);
            services.AddScoped<IPaymentService, StubPaymentService>();

            // Replace IStripeEventParser with a stub
            var parserDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IStripeEventParser));
            if (parserDescriptor is not null)
                services.Remove(parserDescriptor);
            services.AddScoped<IStripeEventParser, StubStripeEventParser>();

            // Replace IImageService with a stub so tests don't hit Cloudinary
            var imageDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IImageService));
            if (imageDescriptor is not null)
                services.Remove(imageDescriptor);
            services.AddScoped<IImageService, StubImageService>();
        });
    }

    // --- Stubs for external services ---

    private sealed class StubPaymentService : IPaymentService
    {
        public Task<PaymentIntent> CreateOrUpdatePaymentIntent(Basket basket) =>
            Task.FromResult(new PaymentIntent
            {
                Id = "pi_test_123",
                ClientSecret = "cs_test_secret"
            });
    }

    private sealed class StubStripeEventParser : IStripeEventParser
    {
        public Event ParseEvent(string json, string signature) =>
            new()
            {
                Data = new EventData
                {
                    Object = new Charge
                    {
                        PaymentIntentId = "pi_test_123",
                        Status = "succeeded"
                    }
                }
            };
    }

    private sealed class StubImageService : IImageService
    {
        public Task<Restore.Core.Results.Result<string>> AddImageAsync(IFormFileService formFileService) =>
            Task.FromResult(Restore.Core.Results.Result<string>.Success("images/products/test.png"));

        public Task<Restore.Core.Results.Result<string>> UpdateImageAsync(IFormFileService formFileService, string pictureUrl) =>
            Task.FromResult(Restore.Core.Results.Result<string>.Success("images/products/test-updated.png"));

        public Task<Restore.Core.Results.Result<bool>> DeleteImageAsync(string imagePath) =>
            Task.FromResult(Restore.Core.Results.Result<bool>.Success(true));
    }
}
