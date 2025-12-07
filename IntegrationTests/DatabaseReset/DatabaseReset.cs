using DotNetEnv;

namespace IntegrationTests.DatabaseReset;

public class DatabaseReset()
{
    [Fact]
    public async Task ResetDatabase()
    {
        Env.Load();
        string ConnectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DATABASE_CONNECTION not found in .env"
            );
        await DatabaseManager.ResetDatabase(ConnectionString);
        await DatabaseManager.SeedDatabase(ConnectionString);
    }
}
