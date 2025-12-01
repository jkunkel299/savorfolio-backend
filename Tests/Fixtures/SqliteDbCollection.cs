namespace Tests.Fixtures;

[CollectionDefinition("SQLite Test database collection")]
public class SqliteDatabaseCollection : ICollectionFixture<SqliteDbFixture>
{
    // marker class for xUnit
}
