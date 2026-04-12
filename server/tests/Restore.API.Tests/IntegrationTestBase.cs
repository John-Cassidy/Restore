using System.Net.Http.Headers;
using System.Net.Http.Json;
using Restore.API.Tests.Fixtures;

namespace Restore.API.Tests;

[Collection("Integration")]
public abstract class IntegrationTestBase : IDisposable
{
    protected readonly CustomWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(PostgresContainerFixture postgres)
    {
        Factory = new CustomWebApplicationFactory(postgres);
        Client = Factory.CreateClient();
    }

    /// <summary>
    /// Logs in as the seeded "bob" user and sets the Authorization header.
    /// </summary>
    protected async Task AuthenticateAsync(string username = "bob", string password = "Admin_1234")
    {
        var response = await Client.PostAsJsonAsync("/api/account/login",
            new { Username = username, Password = password });
        response.EnsureSuccessStatusCode();

        var user = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", user!.Token);
    }

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }

    protected record LoginResponse(string Email, string Token);
}
