namespace IntegrationTests.Fixtures;

[CollectionDefinition("Integration Test Server")]
public class IntegrationTestCollection
    : ICollectionFixture<TestServerFixture>,
        ICollectionFixture<DatabaseFixture>
{
    // marker class for xUnit
}
