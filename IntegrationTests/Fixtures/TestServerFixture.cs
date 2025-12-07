using System.Net;
using System.Net.Http.Json;
using DotNetEnv;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using savorfolio_backend.Data;

namespace IntegrationTests.Fixtures;

public class TestServerFixture : IDisposable
{
    public string ConnectionString { get; set; }
    public HttpClient UnauthenticatedClient { get; private set; }
    public HttpClient AuthenticatedClient { get; private set; }
    private readonly WebApplicationFactory<Program> _factory;

    public TestServerFixture()
    {
        Env.Load();
        ConnectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__DATABASE_CONNECTION")
            ?? throw new InvalidOperationException(
                "ConnectionStrings__DATABASE_CONNECTION not found in .env"
            );

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the production DbContext registration
                var descriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>)
                );
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<AppDbContext>(options => options.UseNpgsql(ConnectionString));
            });
        });

        UnauthenticatedClient = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true,
            }
        );

        AuthenticatedClient = _factory.CreateClient(
            new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                HandleCookies = true,
            }
        );
        AuthenticateAsync().GetAwaiter().GetResult();
    }

    private async Task AuthenticateAsync()
    {
        var credentials = new
        {
            Email = "capstoneUser@savorfolio.com",
            Password = "Savorfolio2025!",
        };

        // Post login with the AuthenticatedClient (same instance that will be used later)
        var loginResponse = await AuthenticatedClient.PostAsJsonAsync(
            "/api/auth/login",
            credentials
        );

        // Diagnostic: make sure login actually set a cookie
        if (!loginResponse.Headers.TryGetValues("Set-Cookie", out var setCookieHeaders))
        {
            // helpful failure message for debugging
            var body = await loginResponse.Content.ReadAsStringAsync();
            throw new InvalidOperationException(
                $"Login did not return Set-Cookie header. Status: {loginResponse.StatusCode}. Body: {body}"
            );
        }

        loginResponse.EnsureSuccessStatusCode();
        // cookie is now stored in AuthenticatedClient's cookie container
    }

    public void Dispose()
    {
        UnauthenticatedClient.Dispose();
        _factory.Dispose();

        GC.SuppressFinalize(this);
    }
}
