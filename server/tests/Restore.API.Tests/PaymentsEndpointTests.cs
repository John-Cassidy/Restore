using System.Net;
using System.Net.Http.Json;
using Restore.API.Tests.Fixtures;

namespace Restore.API.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class PaymentsEndpointTests : IntegrationTestBase
{
    public PaymentsEndpointTests(PostgresContainerFixture postgres) : base(postgres) { }

    [Fact]
    public async Task CreatePaymentIntent_Unauthenticated_Returns401()
    {
        var response = await Client.PostAsync("/api/payments", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreatePaymentIntent_WithBasket_ReturnsOk()
    {
        await AuthenticateAsync();

        // Add item to basket
        var addResponse = await Client.PostAsync("/api/basket?productId=1&quantity=1", null);
        Assert.True(addResponse.IsSuccessStatusCode, "Failed to add item to basket");

        // Create payment intent
        var response = await Client.PostAsync("/api/payments", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
