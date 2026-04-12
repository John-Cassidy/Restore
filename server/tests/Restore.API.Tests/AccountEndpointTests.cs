using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Restore.API.Tests.Fixtures;

namespace Restore.API.Tests;

[Collection("Integration")]
[Trait("Category", "Integration")]
public class AccountEndpointTests : IntegrationTestBase
{
    public AccountEndpointTests(PostgresContainerFixture postgres) : base(postgres) { }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOkWithToken()
    {
        var response = await Client.PostAsJsonAsync("/api/account/login",
            new { Username = "bob", Password = "Admin_1234" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("token", out var token));
        Assert.False(string.IsNullOrEmpty(token.GetString()));
        Assert.True(json.TryGetProperty("email", out var email));
        Assert.Equal("bob@test.com", email.GetString());
    }

    [Fact]
    public async Task Login_WithInvalidPassword_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/api/account/login",
            new { Username = "bob", Password = "wrong" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_Unauthenticated_Returns401()
    {
        var response = await Client.GetAsync("/api/account/current");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetCurrentUser_Authenticated_ReturnsUser()
    {
        await AuthenticateAsync();

        var response = await Client.GetAsync("/api/account/current");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(json.TryGetProperty("email", out var email));
        Assert.Equal("bob@test.com", email.GetString());
    }

    [Fact]
    public async Task Register_WithNewUser_ReturnsCreated()
    {
        var response = await Client.PostAsJsonAsync("/api/account/register",
            new { Username = "newuser", Password = "Test_1234", Email = "newuser@test.com" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithExistingUsername_Returns400()
    {
        var response = await Client.PostAsJsonAsync("/api/account/register",
            new { Username = "bob", Password = "Test_1234", Email = "bob2@test.com" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
