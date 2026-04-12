using System.Net;
using System.Net.Http.Json;
using Restore.API.Tests.Fixtures;

namespace Restore.API.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class BasketEndpointTests : IntegrationTestBase
{
    public BasketEndpointTests(PostgresContainerFixture postgres) : base(postgres) { }

    [Fact]
    public async Task GetBasket_WithNoBuyerCookie_ReturnsSuccessfully()
    {
        // When no buyerId cookie exists, GetBuyerId generates a new GUID
        // and the endpoint responds (may return empty basket or 404 depending on handler)
        using var freshClient = Factory.CreateClient();
        var response = await freshClient.GetAsync("/api/basket");

        Assert.True(
            response.StatusCode is HttpStatusCode.OK or HttpStatusCode.NotFound,
            $"Expected OK or NotFound but got {response.StatusCode}");
    }

    [Fact]
    public async Task AddItemToBasket_CreatesBasketAndReturns201()
    {
        var response = await Client.PostAsync("/api/basket?productId=1&quantity=2", null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var basket = await response.Content.ReadFromJsonAsync<BasketDto>();
        Assert.NotNull(basket);
        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items[0].Quantity);
    }

    [Fact]
    public async Task GetBasket_AfterAddingItem_ReturnsBasket()
    {
        await Client.PostAsync("/api/basket?productId=1&quantity=1", null);

        var response = await Client.GetAsync("/api/basket");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var basket = await response.Content.ReadFromJsonAsync<BasketDto>();
        Assert.NotNull(basket);
        Assert.NotEmpty(basket.Items);
    }

    [Fact]
    public async Task RemoveItemFromBasket_AfterAddingItem_ReturnsOk()
    {
        await Client.PostAsync("/api/basket?productId=2&quantity=3", null);

        var response = await Client.DeleteAsync("/api/basket?productId=2&quantity=1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private record BasketItemDto(int ProductId, string Name, long Price, string PictureUrl, string Brand, string Type, int Quantity);
    private record BasketDto(int Id, string BuyerId, List<BasketItemDto> Items);
}
