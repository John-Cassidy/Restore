using Testcontainers.PostgreSql;

namespace Restore.API.Tests.Fixtures;

public class PostgresContainerFixture : IAsyncLifetime
{
    public PostgreSqlContainer Container { get; } = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("store_test")
        .WithUsername("testuser")
        .WithPassword("testpassword")
        .Build();

    public string ConnectionString => Container.GetConnectionString();

    public async Task InitializeAsync()
    {
        await Container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await Container.DisposeAsync();
    }
}
