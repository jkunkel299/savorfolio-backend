using Microsoft.AspNetCore.Mvc.Testing;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using savorfolio_backend.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Fixtures;

public class TestServerFixture : IDisposable
{
    public string ConnectionString { get; set; }
    public HttpClient HttpClient { get; private set; }
    private readonly WebApplicationFactory<Program> _factory;

    public TestServerFixture()
    {
        Env.Load();
        ConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION")
            ?? throw new InvalidOperationException("ConnectionStrings__DATABASE_CONNECTION not found in .env");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Remove the production DbContext registration
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<AppDbContext>(options => options.UseNpgsql(ConnectionString));
                });
            });
        HttpClient = _factory.CreateClient();
    }

    public void Dispose()
    {
        HttpClient.Dispose();
        _factory.Dispose();

        GC.SuppressFinalize(this);
    }
}