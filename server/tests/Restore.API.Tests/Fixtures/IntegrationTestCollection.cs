namespace Restore.API.Tests.Fixtures;

[CollectionDefinition("Integration")]
public class IntegrationTestCollection : ICollectionFixture<PostgresContainerFixture>
{
}
