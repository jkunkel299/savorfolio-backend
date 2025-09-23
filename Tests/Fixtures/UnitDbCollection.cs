namespace Tests.Fixtures;

[CollectionDefinition("Unit Test database collection")]
public class DatabaseCollection : ICollectionFixture<UnitDbFixture>
{
    // marker class for xUnit
}