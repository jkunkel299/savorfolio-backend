using System.Text.Json;
using DotNetEnv;
using IntegrationTests.DatabaseReset;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using savorfolio_backend.Data;
using Tests.Helpers;

namespace IntegrationTests.Fixtures;

public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    public AppDbContext Context { get; }

    public DatabaseFixture()
    {
        // Add connection string from environment variables
        Env.Load();
        ConnectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DATABASE_CONNECTION not found in .env"
            );

        // Configure TestDbContext
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(ConnectionString)
            .Options;

        Context = new AppDbContext(options);

        // Reset and seed database when fixture is created
        DatabaseManager.ResetDatabase(ConnectionString).Wait();
        DatabaseManager.SeedDatabase(ConnectionString).Wait();
    }

    public void Dispose() //async ValueTask DisposeAsync
    {
        DatabaseManager.ResetDatabase(ConnectionString).Wait();
        DatabaseManager.SeedDatabase(ConnectionString).Wait();
        GC.SuppressFinalize(this);
    }
}
