using AuthService.IntegrationTests.TestServer;

using Grpc.Core;

using Microsoft.Extensions.DependencyInjection;

using Moq;

using Movify;

namespace AuthService.IntegrationTests.TestBase;

public abstract class IntegrationTestBase : IDisposable
{
    private readonly AuthIntegrationTestService _server;

    private readonly Mock<ContentService.ContentServiceClient> _mockContentClient;

    protected IntegrationTestBase()
    {
        var services = new ServiceCollection();

        _mockContentClient = new Mock<ContentService.ContentServiceClient>();

        services.AddSingleton(_mockContentClient.Object);

        _server = new AuthIntegrationTestService(services);
    }

    protected Movify.AuthService.AuthServiceClient GetClient() => new(_server.GrpcChannel);

    protected void SetupMockContentClientReturns(UserRoleResponse userRoleResponse)
    {
        var asyncCall = GrpcTestHelpers.CreateAsyncUnaryCall(userRoleResponse);
        _mockContentClient
            .Setup(
                client => client.GetUserRoleAsync(
                    It.IsAny<UserRoleRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(asyncCall);
    }

    protected void SetupMockContentClientThrows(Exception exception)
    {
        _mockContentClient
            .Setup(
                client => client.GetUserRoleAsync(
                    It.IsAny<UserRoleRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(exception);
    }

    protected void VerifyMockContentClient(UserRoleRequest userRoleRequest)
    {
        _mockContentClient.Verify(
            client => client.GetUserRoleAsync(
                userRoleRequest,
                It.IsAny<Metadata>(),
                It.IsAny<DateTime?>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );
    }

    public void Dispose() => _server.Dispose();
}

public static class GrpcTestHelpers
{
    public static AsyncUnaryCall<TResponse> CreateAsyncUnaryCall<TResponse>(TResponse response)
    {
        return new AsyncUnaryCall<TResponse>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => [],
            () => { }
        );
    }
}
