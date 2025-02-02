using AuthService.Grpc;

using Grpc.Core;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Moq;

using Movify;

namespace AuthService.Tests.Grpc;

[TestSubject(typeof(ContentGrpcClient))]
public class ContentGrpcClientTest
{
    private readonly ContentGrpcClient _client;

    private readonly Mock<ILogger<ContentGrpcClient>> _loggerMock = new();
    private readonly Mock<ContentService.ContentServiceClient> _contentClientMock = new();

    public ContentGrpcClientTest() => _client = new ContentGrpcClient(_loggerMock.Object, _contentClientMock.Object);

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task GetUserRoleAsync_WithNullOrWhiteSpaceEmail_ThrowsArgumentException(string invalidEmail)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => _client.GetUserRoleAsync(invalidEmail));
        Assert.Equal("Email cannot be null or empty (Parameter 'email')", exception.Message);
    }

    [Fact]
    public async Task GetUserRoleAsync_WithSuccessGrpcCall_ReturnsRole()
    {
        // Arrange
        const string email = "test@example.com";
        const string expectedRole = "ADMIN";

        _contentClientMock
            .Setup(
                client => client.GetUserRoleAsync(
                    It.IsAny<UserRoleRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                new AsyncUnaryCall<UserRoleResponse>(
                    Task.FromResult(new UserRoleResponse { Role = expectedRole }),
                    Task.FromResult(new Metadata()),
                    () => new Status(StatusCode.OK, string.Empty),
                    () => [],
                    () => { }
                )
            );

        // Act
        var actualRole = await _client.GetUserRoleAsync(email);

        // Assert
        Assert.Equal(expectedRole, actualRole);

        _contentClientMock
            .Verify(
                client => client.GetUserRoleAsync(
                    It.Is<UserRoleRequest>(r => r.Email == email),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
    }

    [Fact]
    public async Task GetUserRoleAsync_WithFailedGrpcCall_ThrowsRpcException()
    {
        // Arrange
        const string email = "test@example.com";

        _contentClientMock
            .Setup(
                client => client.GetUserRoleAsync(
                    It.IsAny<UserRoleRequest>(),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Throws(new RpcException(new Status(StatusCode.Internal, "Internal server error")));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<RpcException>(() => _client.GetUserRoleAsync(email));
        Assert.Equal(StatusCode.Internal, exception.Status.StatusCode);
        Assert.Equal("Internal server error", exception.Status.Detail);
        
        _contentClientMock
            .Verify(
                client => client.GetUserRoleAsync(
                    It.Is<UserRoleRequest>(r => r.Email == email),
                    It.IsAny<Metadata>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
    }
}
