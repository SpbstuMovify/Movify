using Grpc.Net.Client;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AuthService.IntegrationTests.TestServer;

public class AuthIntegrationTestService : IDisposable
{
    private readonly CustomWebApplicationFactory<Program> _factory;
    public GrpcChannel GrpcChannel { get; private set; }

    public AuthIntegrationTestService(IServiceCollection additionalServices)
    {
        _factory = new CustomWebApplicationFactory<Program>(additionalServices);

        var client = _factory.CreateDefaultClient();

        GrpcChannel = GrpcChannel.ForAddress(
            client.BaseAddress ?? throw new InvalidOperationException(),
            new GrpcChannelOptions
            {
                HttpClient = client
            }
        );
    }

    public void Dispose() => _factory.Dispose();
}

internal class CustomWebApplicationFactory<TEntryPoint>(IServiceCollection mockServices) : WebApplicationFactory<TEntryPoint>
    where TEntryPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services => { services.Add(mockServices); });
    }
}
