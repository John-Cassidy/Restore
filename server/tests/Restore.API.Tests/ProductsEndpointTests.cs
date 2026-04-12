using System.Net;
using System.Net.Http.Json;
using Restore.API.Tests.Fixtures;

namespace Restore.API.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class ProductsEndpointTests : IntegrationTestBase
{
    public ProductsEndpointTests(PostgresContainerFixture postgres) : base(postgres) { }

    [Fact]
    public async Task GetProducts_ReturnsOkWithSeededProducts()
    {
        var response = await Client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task GetProducts_ReturnsPaginationHeader()
    {
        var response = await Client.GetAsync("/api/products");

        Assert.True(response.Headers.Contains("pagination"));
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsOk()
    {
        var response = await Client.GetAsync("/api/products/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetProductById_WithNonExistentId_ReturnsNotFoundOrBadRequest()
    {
        var response = await Client.GetAsync("/api/products/99999");

        // Endpoint may return 404 (null result) or 400 (exception path)
        Assert.True(
            response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.BadRequest,
            $"Expected NotFound or BadRequest but got {response.StatusCode}");
    }

    [Fact]
    public async Task GetProductById_WithNegativeId_ReturnsBadRequest()
    {
        var response = await Client.GetAsync("/api/products/-1");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetProductFilters_ReturnsOkWithBrandsAndTypes()
    {
        var response = await Client.GetAsync("/api/products/filters");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var filters = await response.Content.ReadFromJsonAsync<FiltersResponse>();
        Assert.NotNull(filters);
        Assert.NotEmpty(filters.Brands);
        Assert.NotEmpty(filters.Types);
    }

    private record FiltersResponse(List<string> Brands, List<string> Types);
}
